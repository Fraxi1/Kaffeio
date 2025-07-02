# Sicurezza - Kaffeio

## Panoramica

La sicurezza è un aspetto fondamentale del sistema Kaffeio, progettato per proteggere dati sensibili, garantire l'accesso autorizzato e mantenere l'integrità del sistema. Questo documento descrive le misure di sicurezza implementate in tutte le componenti del sistema.

## Autenticazione e Autorizzazione

### JWT (JSON Web Token)

Il sistema utilizza JWT per l'autenticazione e l'autorizzazione degli utenti.

**Caratteristiche implementate:**
- Token firmati con algoritmo HS256
- Scadenza configurabile dei token (default: 24 ore)
- Refresh token per rinnovare l'accesso senza richiedere nuovamente le credenziali
- Blacklisting dei token revocati

**Esempio di implementazione:**

```typescript
// Generazione del token
@Injectable()
export class AuthService {
  constructor(
    private readonly jwtService: JwtService,
    private readonly usersService: UsersService,
  ) {}

  async validateUser(email: string, password: string): Promise<any> {
    const user = await this.usersService.findByEmail(email);
    if (user && await bcrypt.compare(password, user.password)) {
      const { password, ...result } = user;
      return result;
    }
    return null;
  }

  async login(user: any) {
    const payload = { email: user.email, sub: user.id, roles: user.roles };
    return {
      access_token: this.jwtService.sign(payload),
    };
  }
}
```

```typescript
// Protezione delle rotte
@Controller('users')
export class UsersController {
  constructor(private readonly usersService: UsersService) {}

  @Get()
  @UseGuards(JwtAuthGuard, RolesGuard)
  @Roles('admin')
  findAll() {
    return this.usersService.findAll();
  }
}
```

### Ruoli e Permessi

Il sistema implementa un controllo degli accessi basato su ruoli (RBAC) per garantire che gli utenti possano accedere solo alle risorse per cui sono autorizzati.

**Ruoli implementati:**
- **Admin**: Accesso completo a tutte le funzionalità
- **Manager**: Gestione di strutture, macchine e lotti
- **Operator**: Visualizzazione di dati e monitoraggio
- **Viewer**: Solo visualizzazione di dati in sola lettura

**Esempio di implementazione:**

```typescript
@Injectable()
export class RolesGuard implements CanActivate {
  constructor(private reflector: Reflector) {}

  canActivate(context: ExecutionContext): boolean {
    const requiredRoles = this.reflector.getAllAndOverride<string[]>('roles', [
      context.getHandler(),
      context.getClass(),
    ]);
    if (!requiredRoles) {
      return true;
    }
    const { user } = context.switchToHttp().getRequest();
    return requiredRoles.some((role) => user.roles?.includes(role));
  }
}
```

## Protezione dei Dati

### Crittografia delle Password

Tutte le password degli utenti sono criptate utilizzando bcrypt con un fattore di costo configurabile.

**Esempio di implementazione:**

```typescript
@Injectable()
export class UsersService {
  constructor(
    @InjectRepository(User)
    private usersRepository: Repository<User>,
  ) {}

  async create(createUserDto: CreateUserDto): Promise<User> {
    const hashedPassword = await bcrypt.hash(createUserDto.password, 12);
    const user = this.usersRepository.create({
      ...createUserDto,
      password: hashedPassword,
    });
    return this.usersRepository.save(user);
  }
}
```

### Protezione dei Dati in Transito

Tutte le comunicazioni tra client e server sono protette utilizzando HTTPS/TLS.

**Configurazione del server:**

```typescript
// main.ts
async function bootstrap() {
  const app = await NestFactory.create(AppModule, {
    httpsOptions: {
      key: fs.readFileSync('path/to/private-key.pem'),
      cert: fs.readFileSync('path/to/certificate.pem'),
    },
  });
  // ...
  await app.listen(3000);
}
bootstrap();
```

### Protezione dei Dati a Riposo

I dati sensibili memorizzati nel database sono criptati utilizzando algoritmi di crittografia standard.

**Esempio di implementazione:**

```typescript
@Entity()
export class Customer {
  @Column()
  name: string;

  @Column({ transformer: new EncryptionTransformer({
    key: process.env.ENCRYPTION_KEY,
    algorithm: 'aes-256-cbc',
    ivLength: 16,
  })})
  taxCode: string;

  // ...
}
```

## Sanitizzazione degli Input

Tutti gli input degli utenti sono sanitizzati per prevenire attacchi di injection e XSS.

### Validazione dei DTO

```typescript
export class CreateUserDto {
  @IsEmail()
  @IsNotEmpty()
  email: string;

  @IsString()
  @MinLength(8)
  @MaxLength(20)
  @Matches(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$/, {
    message: 'Password troppo debole',
  })
  password: string;

  @IsString()
  @IsNotEmpty()
  firstName: string;

  @IsString()
  @IsNotEmpty()
  lastName: string;
}
```

### Middleware di Sanitizzazione

```typescript
@Injectable()
export class SanitizationMiddleware implements NestMiddleware {
  use(req: Request, res: Response, next: NextFunction) {
    req.body = sanitize(req.body);
    next();
  }
}
```

## Protezione contro Attacchi Comuni

### Rate Limiting

Il sistema implementa un rate limiting per prevenire attacchi di forza bruta e DoS.

```typescript
// main.ts
async function bootstrap() {
  const app = await NestFactory.create(AppModule);
  
  app.use(
    rateLimit({
      windowMs: 15 * 60 * 1000, // 15 minuti
      max: 100, // limite di 100 richieste per IP
    }),
  );
  
  await app.listen(3000);
}
bootstrap();
```

### CORS (Cross-Origin Resource Sharing)

Il sistema implementa una configurazione CORS sicura per prevenire richieste non autorizzate da domini esterni.

```typescript
// main.ts
async function bootstrap() {
  const app = await NestFactory.create(AppModule);
  
  app.enableCors({
    origin: ['https://your-frontend-domain.com'],
    methods: ['GET', 'POST', 'PUT', 'DELETE'],
    credentials: true,
  });
  
  await app.listen(3000);
}
bootstrap();
```

### Protezione contro CSRF (Cross-Site Request Forgery)

Il sistema implementa token CSRF per prevenire attacchi CSRF.

```typescript
// main.ts
async function bootstrap() {
  const app = await NestFactory.create(AppModule);
  
  app.use(csurf());
  
  await app.listen(3000);
}
bootstrap();
```

### Headers di Sicurezza

Il sistema configura headers HTTP di sicurezza per migliorare la protezione del browser.

```typescript
// main.ts
async function bootstrap() {
  const app = await NestFactory.create(AppModule);
  
  app.use(helmet());
  
  await app.listen(3000);
}
bootstrap();
```

## Logging e Monitoraggio

### Audit Trail

Il sistema mantiene un audit trail completo di tutte le azioni degli utenti per scopi di sicurezza e conformità.

```typescript
@Injectable()
export class AuditService {
  constructor(
    @InjectRepository(AuditLog)
    private auditLogRepository: Repository<AuditLog>,
  ) {}

  async logAction(user: User, action: string, resource: string, details: any): Promise<AuditLog> {
    const auditLog = this.auditLogRepository.create({
      userId: user.id,
      action,
      resource,
      details,
      ipAddress: details.ipAddress,
      userAgent: details.userAgent,
      timestamp: new Date(),
    });
    return this.auditLogRepository.save(auditLog);
  }
}
```

### Monitoraggio delle Anomalie

Il sistema monitora continuamente le attività per rilevare comportamenti anomali che potrebbero indicare tentativi di accesso non autorizzato.

```typescript
@Injectable()
export class SecurityMonitoringService {
  constructor(
    private readonly auditService: AuditService,
    private readonly notificationService: NotificationService,
  ) {}

  @Cron('0 * * * *') // Ogni ora
  async detectAnomalies() {
    const anomalies = await this.detectLoginAnomalies();
    if (anomalies.length > 0) {
      await this.notificationService.notifySecurityTeam(
        'Rilevate anomalie di sicurezza',
        { anomalies },
      );
    }
  }

  private async detectLoginAnomalies(): Promise<any[]> {
    // Implementazione della logica di rilevamento delle anomalie
    // ...
    return anomalies;
  }
}
```

## Gestione delle Vulnerabilità

### Dipendenze Sicure

Il sistema utilizza strumenti automatici per verificare le vulnerabilità nelle dipendenze e aggiorna regolarmente le librerie.

```bash
# Verifica delle vulnerabilità
npm audit

# Aggiornamento delle dipendenze vulnerabili
npm audit fix
```

### Scansione del Codice

Il codice viene regolarmente scansionato per identificare potenziali vulnerabilità di sicurezza.

```bash
# Esecuzione di SonarQube per l'analisi del codice
sonar-scanner \
  -Dsonar.projectKey=kaffeio \
  -Dsonar.sources=. \
  -Dsonar.host.url=http://localhost:9000 \
  -Dsonar.login=your-token
```

## Sicurezza in Produzione

### Gestione dei Segreti

I segreti (password, chiavi API, ecc.) sono gestiti in modo sicuro utilizzando variabili d'ambiente o vault dedicati.

```typescript
// app.module.ts
@Module({
  imports: [
    ConfigModule.forRoot({
      isGlobal: true,
      envFilePath: `.env.${process.env.NODE_ENV}`,
      validationSchema: Joi.object({
        DATABASE_URL: Joi.string().required(),
        JWT_SECRET: Joi.string().required(),
        JWT_EXPIRATION_TIME: Joi.string().required(),
        // ...
      }),
    }),
    // ...
  ],
  // ...
})
export class AppModule {}
```

### Backup e Disaster Recovery

Il sistema implementa procedure di backup regolari e un piano di disaster recovery per garantire la continuità operativa in caso di incidenti di sicurezza.

## Best Practices di Sicurezza

1. **Principio del minimo privilegio**: Gli utenti hanno accesso solo alle risorse necessarie per svolgere le loro funzioni
2. **Difesa in profondità**: Implementazione di più livelli di sicurezza per proteggere il sistema
3. **Aggiornamenti regolari**: Tutte le componenti del sistema sono regolarmente aggiornate per correggere vulnerabilità note
4. **Formazione sulla sicurezza**: Gli sviluppatori e gli utenti sono formati sulle best practices di sicurezza
5. **Test di penetrazione**: Il sistema è regolarmente sottoposto a test di penetrazione per identificare e correggere vulnerabilità

## Conformità

Il sistema è progettato per essere conforme ai seguenti standard e regolamenti:

- **GDPR**: Protezione dei dati personali
- **ISO 27001**: Gestione della sicurezza delle informazioni
- **OWASP Top 10**: Protezione contro le vulnerabilità web più comuni
