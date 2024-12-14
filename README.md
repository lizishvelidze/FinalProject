# Loan Management API
The Loan Management API is a system designed to manage loan applications, user registrations, account management for both regular users and accountants, and loan-related operations. It allows users to register, apply for loans, and view their profiles, while accountants can manage loan statuses and block users.

## Features
User Registration & Login: Users and accountants can register and log in.

Loan Management: Accountants can view all loans, update their statuses, and delete loans.

User Profile Management: Both users and accountants can access user profiles.

## Table of Contents
Prerequisites

Installation

Running the Application

API Endpoints

User Endpoints

Accountant Endpoints

Authentication

Testing

License

Prerequisites
## Before getting started, make sure you have the following installed:

.NET (version 5.0 or later)
Postman or another API testing tool (optional)
SQL Server or an alternative database management system (optional, for production environments)

### Installation
To get started with the Loan Management API, follow these steps:

1. **Clone the repository:**
```bash
git clone https://github.com/yourusername/LoanManagementAPI.git
```
2. **Navigate to the project folder:**
```bash
cd LoanManagementAPI
```
3. **Restore dependencies:**
```bash
dotnet restore
```
4. **Run the application:**
```bash
dotnet run
```
The application will start running on http://localhost:5000 by default.

5. **Running the Application**
To start the application, use the command:
```bash
dotnet run
```
This will start the API on http://localhost:5000 (or another port if configured).

API Endpoints
User Endpoints
1. **Register a User**

Method: POST

URL: /api/user/register

Request Body:
```bash
{
  "username": "john_doe",
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "password": "securepassword123",
  "age": 30,
  "monthlyIncome": 3000
}
```
Description: Registers a new user. Returns the user details excluding the password upon successful registration.
2. **Login a User**

Method: POST

URL: /api/user/login

Request Body:
```bash
{
  "username": "john_doe",
  "password": "securepassword123"
}
```
Description: Logs in a user by verifying the credentials and returns a JWT token.

3. **Get User Profile**

Method: GET

URL: /api/user/profile

Authentication: Bearer token

Description: Retrieves the user's profile information, including all associated loans.

4. **Get User By ID**

Method: GET

URL: /api/user/{id}

Authentication: Bearer token

Description: Fetches the profile of a specific user by their ID, including associated loans.

Accountant Endpoints
1. **Register an Accountant**

Method: POST

URL: /api/user/register-accountant

Request Body:
```bash
{
  "username": "accountant_jane",
  "firstName": "Jane",
  "lastName": "Smith",
  "email": "jane.smith@example.com",
  "password": "adminpassword123",
  "age": 40,
  "monthlyIncome": 5000
}
```
Description: Registers a new accountant.

2. **Accountant Login**

Method: POST

URL: /api/user/accountant-login

Request Body:
```bash
{
  "username": "accountant_jane",
  "password": "adminpassword123"
}
```
Description: Logs in an accountant by verifying their credentials and returns a JWT token.

3. **Get All Loans**
 
Method: GET

URL: /api/accountant/all-loans

Authentication: Bearer token (Accountant role required)

Description: Fetches a list of all loans available in the system.

4. **Update Loan Status**

Method: PUT

URL: /api/accountant/loans/{loanId}/update-status

Request Body:
```bash
{
  "status": "Approved"
}
```
Description: Updates the status of a specific loan.

5. **Block User**

Method: PUT

URL: /api/accountant/users/{userId}/block

Request Query Parameter: blockDurationDays=30 (Optional, default is 30 days)

Description: Blocks a user for a given number of days.

##Authentication
###The API uses JWT (JSON Web Tokens) for authentication. To log in, users and accountants must send a POST request to the login endpoints, receiving a token in response. This token should be included in the Authorization header of all subsequent requests, formatted as follows:

```bash

Authorization: Bearer <your-jwt-token>
```
##Testing
###To run the automated tests for this application:

1. Run unit tests:

Run the following command in your terminal:
```bash
dotnet test
```
This will execute all the unit tests and provide feedback on whether the tests passed or failed.

##Built With
.NET Core - The framework used for building the API 

Entity Framework Core - ORM used for database operations

JWT - Authentication using JSON Web Tokens

SQL Server - Database used for storing user and loan information

##License
This project is licensed under the MIT License - see the LICENSE.md file for details.

##Acknowledgments
Hat tip to the developers whose code and ideas helped build this system.

Special thanks to the contributors who reviewed and improved this project.
