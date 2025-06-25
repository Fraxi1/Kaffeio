# Kaffeio API Documentation

This document provides detailed information about the Kaffeio API endpoints, request/response formats, and authentication requirements.

## Table of Contents

- [Authentication](#authentication)
- [Users](#users)
- [Facilities](#facilities)
- [Machines](#machines)
- [Lots](#lots)
- [Telemetry](#telemetry)

## Authentication

Authentication is handled using JWT (JSON Web Tokens).

### Register a New User

```
POST /api/auth/register
```

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "securePassword123",
  "firstName": "John",
  "lastName": "Doe"
}
```

**Response:**
```json
{
  "id": 1,
  "email": "user@example.com",
  "firstName": "John",
  "lastName": "Doe"
}
```

### Login

```
POST /api/auth/login
```

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "securePassword123"
}
```

**Response:**
```json
{
  "access_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe"
  }
}
```

## Users

### Get All Users

```
GET /api/users
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Response:**
```json
[
  {
    "id": 1,
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe"
  },
  {
    "id": 2,
    "email": "user2@example.com",
    "firstName": "Jane",
    "lastName": "Smith"
  }
]
```

### Get User by ID

```
GET /api/users/:id
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Response:**
```json
{
  "id": 1,
  "email": "user@example.com",
  "firstName": "John",
  "lastName": "Doe"
}
```

### Create User

```
POST /api/users
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Request Body:**
```json
{
  "email": "newuser@example.com",
  "password": "securePassword123",
  "firstName": "New",
  "lastName": "User"
}
```

**Response:**
```json
{
  "id": 3,
  "email": "newuser@example.com",
  "firstName": "New",
  "lastName": "User"
}
```

### Update User

```
PUT /api/users/:id
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Request Body:**
```json
{
  "firstName": "Updated",
  "lastName": "Name"
}
```

**Response:**
```json
{
  "id": 1,
  "email": "user@example.com",
  "firstName": "Updated",
  "lastName": "Name"
}
```

### Delete User

```
DELETE /api/users/:id
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Response:**
```json
{
  "message": "User with ID 1 has been deleted"
}
```

## Facilities

### Get All Facilities

```
GET /api/facilities
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Response:**
```json
[
  {
    "id": 1,
    "name": "Main Factory",
    "location": "New York",
    "timeZone": "America/New_York"
  },
  {
    "id": 2,
    "name": "Secondary Factory",
    "location": "Los Angeles",
    "timeZone": "America/Los_Angeles"
  }
]
```

### Get Facility by ID

```
GET /api/facilities/:id
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Response:**
```json
{
  "id": 1,
  "name": "Main Factory",
  "location": "New York",
  "timeZone": "America/New_York"
}
```

### Create Facility

```
POST /api/facilities
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Request Body:**
```json
{
  "name": "New Factory",
  "location": "Chicago",
  "timeZone": "America/Chicago"
}
```

**Response:**
```json
{
  "id": 3,
  "name": "New Factory",
  "location": "Chicago",
  "timeZone": "America/Chicago"
}
```

### Update Facility

```
PUT /api/facilities/:id
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Request Body:**
```json
{
  "name": "Updated Factory Name",
  "location": "Miami",
  "timeZone": "America/New_York"
}
```

**Response:**
```json
{
  "id": 1,
  "name": "Updated Factory Name",
  "location": "Miami",
  "timeZone": "America/New_York"
}
```

### Delete Facility

```
DELETE /api/facilities/:id
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Response:**
```json
{
  "message": "Facility with ID 1 has been deleted"
}
```

## Machines

### Get All Machines

```
GET /api/machines
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Response:**
```json
[
  {
    "id": 1,
    "name": "Machine 1",
    "type": "Roaster",
    "status": "Idle",
    "facilityId": 1,
    "facility": {
      "id": 1,
      "name": "Main Factory",
      "location": "New York",
      "timeZone": "America/New_York"
    }
  },
  {
    "id": 2,
    "name": "Machine 2",
    "type": "Grinder",
    "status": "Running",
    "facilityId": 1,
    "facility": {
      "id": 1,
      "name": "Main Factory",
      "location": "New York",
      "timeZone": "America/New_York"
    }
  }
]
```

### Get Machines by Facility

```
GET /api/machines/facility/:facilityId
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Response:**
```json
[
  {
    "id": 1,
    "name": "Machine 1",
    "type": "Roaster",
    "status": "Idle",
    "facilityId": 1,
    "facility": {
      "id": 1,
      "name": "Main Factory",
      "location": "New York",
      "timeZone": "America/New_York"
    }
  },
  {
    "id": 2,
    "name": "Machine 2",
    "type": "Grinder",
    "status": "Running",
    "facilityId": 1,
    "facility": {
      "id": 1,
      "name": "Main Factory",
      "location": "New York",
      "timeZone": "America/New_York"
    }
  }
]
```

### Get Machine by ID

```
GET /api/machines/:id
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Response:**
```json
{
  "id": 1,
  "name": "Machine 1",
  "type": "Roaster",
  "status": "Idle",
  "facilityId": 1,
  "facility": {
    "id": 1,
    "name": "Main Factory",
    "location": "New York",
    "timeZone": "America/New_York"
  }
}
```

### Create Machine

```
POST /api/machines
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Request Body:**
```json
{
  "name": "New Machine",
  "type": "Packager",
  "facilityId": 1,
  "status": "Idle"
}
```

**Response:**
```json
{
  "id": 3,
  "name": "New Machine",
  "type": "Packager",
  "status": "Idle",
  "facilityId": 1,
  "facility": {
    "id": 1,
    "name": "Main Factory",
    "location": "New York",
    "timeZone": "America/New_York"
  }
}
```

### Update Machine

```
PUT /api/machines/:id
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Request Body:**
```json
{
  "name": "Updated Machine Name",
  "status": "Maintenance"
}
```

**Response:**
```json
{
  "id": 1,
  "name": "Updated Machine Name",
  "type": "Roaster",
  "status": "Maintenance",
  "facilityId": 1,
  "facility": {
    "id": 1,
    "name": "Main Factory",
    "location": "New York",
    "timeZone": "America/New_York"
  }
}
```

### Update Machine Status

```
PATCH /api/machines/:id/status
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Request Body:**
```json
{
  "status": "Running"
}
```

**Response:**
```json
{
  "id": 1,
  "name": "Machine 1",
  "type": "Roaster",
  "status": "Running",
  "facilityId": 1,
  "facility": {
    "id": 1,
    "name": "Main Factory",
    "location": "New York",
    "timeZone": "America/New_York"
  }
}
```

### Delete Machine

```
DELETE /api/machines/:id
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Response:**
```json
{
  "message": "Machine with ID 1 has been deleted"
}
```

## Lots

### Get All Lots

```
GET /api/lots
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Response:**
```json
[
  {
    "Id": 1,
    "Code": "LOT-001",
    "Description": "Arabica beans from Colombia",
    "CreatedAt": "2025-06-25T10:30:00.000Z",
    "Status": "Created",
    "CurrentMachine": {
      "id": 1,
      "name": "Machine 1",
      "type": "Roaster",
      "status": "Running",
      "facilityId": 1
    }
  },
  {
    "Id": 2,
    "Code": "LOT-002",
    "Description": "Robusta beans from Brazil",
    "CreatedAt": "2025-06-25T11:45:00.000Z",
    "Status": "Processing",
    "CurrentMachine": null
  }
]
```

### Get Lot by ID

```
GET /api/lots/:id
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Response:**
```json
{
  "Id": 1,
  "Code": "LOT-001",
  "Description": "Arabica beans from Colombia",
  "CreatedAt": "2025-06-25T10:30:00.000Z",
  "Status": "Created",
  "CurrentMachine": {
    "id": 1,
    "name": "Machine 1",
    "type": "Roaster",
    "status": "Running",
    "facilityId": 1
  }
}
```

### Get Lot by Code

```
GET /api/lots/code/:code
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Response:**
```json
{
  "Id": 1,
  "Code": "LOT-001",
  "Description": "Arabica beans from Colombia",
  "CreatedAt": "2025-06-25T10:30:00.000Z",
  "Status": "Created",
  "CurrentMachine": {
    "id": 1,
    "name": "Machine 1",
    "type": "Roaster",
    "status": "Running",
    "facilityId": 1
  }
}
```

### Create Lot

```
POST /api/lots
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Request Body:**
```json
{
  "Code": "LOT-003",
  "Description": "New coffee lot",
  "Status": "Created",
  "CurrentMachineId": 1
}
```

**Response:**
```json
{
  "Id": 3,
  "Code": "LOT-003",
  "Description": "New coffee lot",
  "CreatedAt": "2025-06-25T14:00:00.000Z",
  "Status": "Created",
  "CurrentMachine": {
    "id": 1,
    "name": "Machine 1",
    "type": "Roaster",
    "status": "Running",
    "facilityId": 1
  }
}
```

### Update Lot

```
PUT /api/lots/:id
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Request Body:**
```json
{
  "Description": "Updated description",
  "Status": "Processing"
}
```

**Response:**
```json
{
  "Id": 1,
  "Code": "LOT-001",
  "Description": "Updated description",
  "CreatedAt": "2025-06-25T10:30:00.000Z",
  "Status": "Processing",
  "CurrentMachine": {
    "id": 1,
    "name": "Machine 1",
    "type": "Roaster",
    "status": "Running",
    "facilityId": 1
  }
}
```

### Delete Lot

```
DELETE /api/lots/:id
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Response:**
```json
{
  "message": "Lot with ID 1 has been deleted"
}
```

## Telemetry

### Get All Telemetry (Paginated)

```
GET /api/telemetry?page=1&limit=100
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Response:**
```json
{
  "data": [
    {
      "id": 1,
      "machineId": 1,
      "lotId": 1,
      "timestamp": "2025-06-25T12:30:00.000Z",
      "dataType": "temperature",
      "dataValue": "195.5",
      "machine": {
        "id": 1,
        "name": "Machine 1",
        "type": "Roaster",
        "status": "Running",
        "facilityId": 1
      },
      "lot": {
        "Id": 1,
        "Code": "LOT-001",
        "Description": "Arabica beans from Colombia",
        "Status": "Processing"
      }
    },
    {
      "id": 2,
      "machineId": 1,
      "lotId": 1,
      "timestamp": "2025-06-25T12:31:00.000Z",
      "dataType": "humidity",
      "dataValue": "45.2",
      "machine": {
        "id": 1,
        "name": "Machine 1",
        "type": "Roaster",
        "status": "Running",
        "facilityId": 1
      },
      "lot": {
        "Id": 1,
        "Code": "LOT-001",
        "Description": "Arabica beans from Colombia",
        "Status": "Processing"
      }
    }
  ],
  "total": 150,
  "page": 1,
  "limit": 100
}
```

### Get Telemetry by ID

```
GET /api/telemetry/:id
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Response:**
```json
{
  "id": 1,
  "machineId": 1,
  "lotId": 1,
  "timestamp": "2025-06-25T12:30:00.000Z",
  "dataType": "temperature",
  "dataValue": "195.5",
  "machine": {
    "id": 1,
    "name": "Machine 1",
    "type": "Roaster",
    "status": "Running",
    "facilityId": 1
  },
  "lot": {
    "Id": 1,
    "Code": "LOT-001",
    "Description": "Arabica beans from Colombia",
    "Status": "Processing"
  }
}
```

### Get Telemetry by Machine (Paginated)

```
GET /api/telemetry/machine/:machineId?page=1&limit=100
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Response:**
```json
{
  "data": [
    {
      "id": 1,
      "machineId": 1,
      "lotId": 1,
      "timestamp": "2025-06-25T12:30:00.000Z",
      "dataType": "temperature",
      "dataValue": "195.5",
      "machine": {
        "id": 1,
        "name": "Machine 1",
        "type": "Roaster",
        "status": "Running",
        "facilityId": 1
      },
      "lot": {
        "Id": 1,
        "Code": "LOT-001",
        "Description": "Arabica beans from Colombia",
        "Status": "Processing"
      }
    }
  ],
  "total": 75,
  "page": 1,
  "limit": 100
}
```

### Get Telemetry by Lot (Paginated)

```
GET /api/telemetry/lot/:lotId?page=1&limit=100
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Response:**
```json
{
  "data": [
    {
      "id": 1,
      "machineId": 1,
      "lotId": 1,
      "timestamp": "2025-06-25T12:30:00.000Z",
      "dataType": "temperature",
      "dataValue": "195.5",
      "machine": {
        "id": 1,
        "name": "Machine 1",
        "type": "Roaster",
        "status": "Running",
        "facilityId": 1
      },
      "lot": {
        "Id": 1,
        "Code": "LOT-001",
        "Description": "Arabica beans from Colombia",
        "Status": "Processing"
      }
    }
  ],
  "total": 50,
  "page": 1,
  "limit": 100
}
```

### Get Telemetry by Date Range (Paginated)

```
GET /api/telemetry/daterange?startDate=2025-06-25T00:00:00.000Z&endDate=2025-06-25T23:59:59.999Z&page=1&limit=100
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Response:**
```json
{
  "data": [
    {
      "id": 1,
      "machineId": 1,
      "lotId": 1,
      "timestamp": "2025-06-25T12:30:00.000Z",
      "dataType": "temperature",
      "dataValue": "195.5",
      "machine": {
        "id": 1,
        "name": "Machine 1",
        "type": "Roaster",
        "status": "Running",
        "facilityId": 1
      },
      "lot": {
        "Id": 1,
        "Code": "LOT-001",
        "Description": "Arabica beans from Colombia",
        "Status": "Processing"
      }
    }
  ],
  "total": 120,
  "page": 1,
  "limit": 100
}
```

### Create Telemetry

```
POST /api/telemetry
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Request Body:**
```json
{
  "machineId": 1,
  "lotId": 1,
  "dataType": "pressure",
  "dataValue": "12.5"
}
```

**Response:**
```json
{
  "id": 3,
  "machineId": 1,
  "lotId": 1,
  "timestamp": "2025-06-25T14:10:00.000Z",
  "dataType": "pressure",
  "dataValue": "12.5",
  "machine": {
    "id": 1,
    "name": "Machine 1",
    "type": "Roaster",
    "status": "Running",
    "facilityId": 1
  },
  "lot": {
    "Id": 1,
    "Code": "LOT-001",
    "Description": "Arabica beans from Colombia",
    "Status": "Processing"
  }
}
```

### Update Telemetry

```
PUT /api/telemetry/:id
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Request Body:**
```json
{
  "dataValue": "13.2"
}
```

**Response:**
```json
{
  "id": 3,
  "machineId": 1,
  "lotId": 1,
  "timestamp": "2025-06-25T14:10:00.000Z",
  "dataType": "pressure",
  "dataValue": "13.2",
  "machine": {
    "id": 1,
    "name": "Machine 1",
    "type": "Roaster",
    "status": "Running",
    "facilityId": 1
  },
  "lot": {
    "Id": 1,
    "Code": "LOT-001",
    "Description": "Arabica beans from Colombia",
    "Status": "Processing"
  }
}
```

### Delete Telemetry

```
DELETE /api/telemetry/:id
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Response:**
```json
{
  "message": "Telemetry with ID 3 has been deleted"
}
```
