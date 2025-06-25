import { Module } from "@nestjs/common";
import { TypeOrmModule } from "@nestjs/typeorm";
import { User } from "../schema/entities/user.entity";
import { UserService } from "../services/user.service";
import { QueueModule } from "./queue.module";

@Module({
    imports: [TypeOrmModule.forFeature([User]), QueueModule],
    providers: [UserService],
    exports: [UserService],
})
export class UserModule { }