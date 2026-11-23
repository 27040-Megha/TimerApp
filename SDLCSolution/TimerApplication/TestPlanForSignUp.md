# Signup & Validation Test Plan

This document outlines the unit test cases for the **Signup** service method (`CreateUser`) and the presentation layer **Email** and **Password** validation helper methods.

---

## 1. Signup Feature (`CreateUser`)

**Input Parameters:** `User name, email, password`

| Scenario / Description | Test Data | Expected Result |
| :--- | :--- | :--- |
| Valid Username, valid email, valid password (New email ID) | `Megha`, `meghaeg@gmail.com`, `Megha@123` | `SUCCESS: Signup successful` |
| Valid Username, valid email, valid password (Email exists already) | `Megha`, `meghaeg@gmail.com`, `Megha@123` | `ERROR: Email ID already exists` |

---

## 2. Email Validation Helper

**Input Parameter:** `email (string)`

| Scenario / Description | Test Data | Expected Result |
| :--- | :--- | :--- |
| Valid Email | `meghaeg@gmail.com`, `megha@kec.edu` | `SUCCESS: Valid Email` |
| Invalid email (Missing @) | `meghagmail.com` | `ERROR: Invalid Email format` |
| Invalid email (Missing characters before @) | `@meghagmail.com` | `ERROR: Invalid Email format` |
| Invalid email (Missing .) | `megha@gmailcom` | `ERROR: Invalid Email format` |
| Invalid email (Missing characters between @ and .) | `megha@.com` | `ERROR: Invalid Email format` |
| Invalid email (Missing characters after .) | `megha@gmail.` | `ERROR: Invalid Email format` |

---

## 3. Password Validation Helper

**Input Parameter:** `Password (string)`

| Scenario / Description | Test Data | Expected Result |
| :--- | :--- | :--- |
| Valid Password | `Megha@123` | `SUCCESS: Valid Password` |
| Invalid Password (Length less than 8) | `Meg@12` | `ERROR: Invalid Password format` |
| Invalid Password (Missing capital letter) | `megha@123` | `ERROR: Invalid Password format` |
| Invalid Password (Missing Special character) | `megha1234` | `ERROR: Invalid Password format` |
| Invalid Password (Missing numbers) | `meghaa@eg` | `ERROR: Invalid Password format` |
