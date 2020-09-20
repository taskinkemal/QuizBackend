# QuizBackend
Quiz Backend

## Registration Workflow
1. [PUT] Users
-> Sends an email with a one-time token.
2. [POST] Users/VerifyAccount {"token":"one time token","deviceId":"unique device Id"}

## Log In
Currently only built-in login functionality is supported.

1. [POST] Token {"email":"your email", "password":"your password","deviceId":"unique device Id"}

## Password Reset

1. [POST] Password/SendPasswordResetEmail {"email":"your email"}
-> Sends an email with a one-time token.
2. [POST] PasswordWithToken {"password":"new password","token":"one time token","deviceId":"unique device Id"}

## Password Update
1. [POST] Password {"password":"new password","deviceId":"unique device Id"} (incl. auth token)

Password helper functions:
- [GET] Password -> returns password criteria
- [POST] Password/ValidatePassword -> validates password against password criteria.
