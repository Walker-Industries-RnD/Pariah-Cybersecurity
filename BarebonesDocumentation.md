# Barebones Documentation

## Create An Account

### HTTP Request



| Parameter | Type   | Description                |
|-----------|--------|----------------------------|
| Username  | string | **Required**. The username |
| Password  | string | **Required**. The password |

Creates a new user account by:
1. Adding a salt to the password.
2. Encrypting the combined string with Scrypt.
3. Saving the password (with Scrypt) and salt (with AES).

---

## Login

### HTTP Request


| Parameter | Type   | Description                |
|-----------|--------|----------------------------|
| Username  | string | **Required**. The username |
| Password  | string | **Required**. The password |

Logs the user in by:
1. Checking if the entered password decrypts the AES-encrypted salt.
2. Comparing the decrypted result with the Scrypt-encrypted password.
3. Setting the following static variables:
   - `GlobalPass` (Password)
   - `TempDecryptedFilePath` (Temporary file path for decrypted files)
   - `DataPath` (Path to user data)
   - `User` (Logged in user)

---

## Classes and Methods

### Class: PariahCybersec

#### Static Fields

| Field                  | Description                                       |
|------------------------|---------------------------------------------------|
| `GlobalPass`           | Password                                          |
| `TempDecryptedFilePath`| Temporary file path for decrypted files          |
| `DataPath`             | Path to user data                                |
| `User`                 | Logged in user                                   |

### Class: PasswordGenerator

| Method                                | Parameters                                    | Description                                         |
|---------------------------------------|----------------------------------------------|-----------------------------------------------------|
| `CreatePassword`                      | `length: int`, `IsNumeric: bool`, `SpecialCharacters: bool`, `Caps: bool`, `Lowercase: bool` | Generates a random password based on specified criteria |
| `GetNumeric`                          | -                                            | Returns a random numeric character                 |
| `GetUppercase`                        | -                                            | Returns a random uppercase character               |
| `GetLowercase`                        | -                                            | Returns a random lowercase character               |
| `GetSpecial`                          | -                                            | Returns a random special character                 |

### Class: EncryptedBoolGenerator

| Method                                 | Parameters                                    | Description                                         |
|----------------------------------------|----------------------------------------------|-----------------------------------------------------|
| `DecryptBoolGroup`                     | `encryptedgroup: EncryptedBoolGroup`          | Decrypts an `EncryptedBoolGroup` into `BoolGroup`  |
| `EditBoolNameInGroup`                  | `Name: string`, `NewName: string`, `NewVal: bool`, `encryptedgroup: EncryptedBoolGroup` | Edits the name of a boolean in an `EncryptedBoolGroup` |
| `AddBoolToGroup`                       | `NewName: string`, `NewVal: bool`, `encryptedgroup: EncryptedBoolGroup` | Adds a new boolean value to an `EncryptedBoolGroup` |
| `RemoveBoolNameInGroup`                | `Name: string`, `NewVal: bool`, `encryptedgroup: EncryptedBoolGroup` | Removes a boolean value from an `EncryptedBoolGroup` |
| `EditBoolInGroup`                      | `Name: string`, `NewVal: bool`, `encryptedgroup: EncryptedBoolGroup` | Edits a boolean value in an `EncryptedBoolGroup` |
| `GetBoolValue`                         | `Name: string`, `encryptedgroup: EncryptedBoolGroup` | Gets the boolean value for a specific name from an `EncryptedBoolGroup` |

### Class: MainPasswordHandler

| Method                     | Parameters       | Description                                |
|----------------------------|------------------|--------------------------------------------|
| `EncryptPassword`          | `Input: string`  | Encrypts a password with Scrypt and AES   |

### Class: AccountEncryptor

| Struct                      | Field                          | Description                                |
|-----------------------------|--------------------------------|--------------------------------------------|
| `EncryptedAccountData`      | `Email`, `Username`, `Password`, `Has2FA`, `AuthProvider`, `CreatedOn`, `LastEdited`, `LastPassChange`, `Website`, `Notes`, `MainLogoPath`, `FilterType` | Holds encrypted account data                |
| `AccountData`               | `Email`, `Username`, `Password`, `Has2FA`, `AuthProvider`, `CreatedOn`, `LastEdited`, `LastPassChange`, `Website`, `Notes`, `MainLogoPath`, `FilterType` | Holds raw account data                      |

| Method                          | Parameters                               | Description                                         |
|---------------------------------|------------------------------------------|-----------------------------------------------------|
| `DecryptAccountData`             | `item: EncryptedAccountData`             | Decrypts `EncryptedAccountData` into `AccountData` |

### Class: FileEncrypter

| Method                                | Parameters                      | Description                                   |
|---------------------------------------|--------------------------------|-----------------------------------------------|
| `CheckIfFileCanBeEncrypted`           | `RequestedPath: string`         | Checks if a file can be encrypted            |
| `EncryptFileAsync`                    | `RequestedPath: string`, `DeleteOriginalFile: bool` | Encrypts a file asynchronously              |
| `DecryptFileAsync`                    | `item: SimpleAESEncryption.AESEncryptedText` | Decrypts a file asynchronously              |
| `RewriteFileIntoExistenceAsync`       | `base64String: string`, `filename: string`, `extension: string`, `directory: string` | Writes a decrypted file into existence asynchronously |

### Class: Utilities

| Method                      | Parameters | Description                          |
|-----------------------------|------------|--------------------------------------|
| `CreateUUID`                | -          | Creates a UUID                        |
| `GetRandomLengthString`     | -          | Generates a random length string      |

