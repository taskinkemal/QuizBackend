![Build Status](https://github.com/taskinkemal/QuizBackend/workflows/.NET%20Core/badge.svg?branch=master) [![Coverage Status](https://coveralls.io/repos/github/taskinkemal/QuizBackend/badge.svg?branch=github-actions)](https://coveralls.io/github/taskinkemal/QuizBackend?branch=master)

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

## Quizzes

1. [GET] Questions/Quiz
-> returns a list of quizzes that the user is assigned to.
-> Id is the effective quiz id. The field QuizId is the quiz identity id, please ignore this.
2. [GET] Questions/Quiz/{Quiz Id}
-> returns a list of questions of the given quiz. This includes the option id's for each question. Questions are ordered.
3. [GET] Options/Quiz/{Quiz Id}
-> returns a list of options of the given quiz. Options are ordered.
