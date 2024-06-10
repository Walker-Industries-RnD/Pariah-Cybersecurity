
<pre>

    ____             _       __       ______      __                                        _ __       
   / __ \____ ______(_)___ _/ /_     / ____/_  __/ /_  ___  _____________  _______  _______(_) /___  __
  / /_/ / __ `/ ___/ / __ `/ __ \   / /   / / / / __ \/ _ \/ ___/ ___/ _ \/ ___/ / / / ___/ / __/ / / /
 / ____/ /_/ / /  / / /_/ / / / /  / /___/ /_/ / /_/ /  __/ /  (__  )  __/ /__/ /_/ / /  / / /_/ /_/ / 
/_/    \__,_/_/  /_/\__,_/_/ /_/   \____/\__, /_.___/\___/_/  /____/\___/\___/\__,_/_/  /_/\__/\__, /  
                                        /____/                                                /____/  
          .                                                      .
        .n                   .                 .                  n.
  .   .dP                  dP                   9b                 9b.    .
 4    qXb         .       dX                     Xb       .        dXp     t
dX.    9Xb      .dXb    __                         __    dXb.     dXP     .Xb
9XXb._       _.dXXXXb dXXXXbo.                 .odXXXXb dXXXXb._       _.dXXP
 9XXXXXXXXXXXXXXXXXXXVXXXXXXXXOo.           .oOXXXXXXXXVXXXXXXXXXXXXXXXXXXXP
  `9XXXXXXXXXXXXXXXXXXXXX'~   ~`OOO8b   d8OOO'~   ~`XXXXXXXXXXXXXXXXXXXXXP'
    `9XXXXXXXXXXXP' `9XX'          `98v8P'          `XXP' `9XXXXXXXXXXXP'
        ~~~~~~~       9X.          .db|db.          .XP       ~~~~~~~
                        )b.  .dbo.dP'`v'`9b.odb.  .dX(
                      ,dXXXXXXXXXXXb     dXXXXXXXXXXXb.
                     dXXXXXXXXXXXP'   .   `9XXXXXXXXXXXb
                    dXXXXXXXXXXXXb   d|b   dXXXXXXXXXXXXb
                    9XXb'   `XXXXXb.dX|Xb.dXXXXX'   `dXXP
                     `'      9XXXXXX(   )XXXXXXP      `'
                              XXXX X.`v'.X XXXX
                              XP^X'`b   d'`X^XX
                              X. 9  `   '  P )X
                              `b  `       '  d'
                               `             '

</pre>



## Easy, Open Sourced and AES256 and Scrypt Based Cybersecurity

#### Pariah Cybersecurity is a C# Cybersecurity system. Primarily made for the XRUIOS (Coming soon) and Unity. It should be able to work with non Unity projects with a few changes to the code.



     
## Pariah Is Composed Of

- A Secure Local Sign In and Sign Up System Built Atop Scrypt
- An AES-256 Based Password Manager 
- An AES-256 Based File Encrypter
- A (Close To) True Random Password Generator 

And much, much more to come!
## Installation

First, ensure you install the proper prerequisite projects!


[[UniversalSave]](https://github.com/LifeandStyleMedia/UniversalSave)

[[SimpleAESEncryption]](https://github.com/dubit/unity-crypto)

[[Scrypt]](https://github.com/viniciuschiele/Scrypt)


*For Universal Save, get UniversalSave_1_0_2_UVS.unitypackage and you can have UVS at the BG if you want, also be sure to go to UniversalSave.cs and switch the default to JSON with (public DataFormat dataFormat = DataFormat.JSON;)! I might switch back to nodes later if the files feel like they will be huge!*


From there, you can insert the C# file named "Pariah Cybersecurity" to Unity. If you would rather make your own save system, I reccomend using the file names "Pariah Barebones"!
## Scripting Reference

#### Create An Account

```http
  AccountSystem.CreateAcc()
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `Username` | `string` | **Required**. The username |
| `Password` | `string` | **Required**. The password |

Creates a new user account while adding a salt to the password, encrypting the entire string with Scrypt and saving both (Password with Scrypt, Salt with AES). 


#### Login
```http
  AccountSystem.Login()
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `Username` | `string` | **Required**. The username|
| `Password` | `string` | **Required**. The password |

Logs the user in by checking if the entered password can decrypt the AES encrypted salt, then adds to password and compares with Scrypt to determine if user. Also sets the static variables;

- GlobalPass, or Password

- TempDecryptedFilePath, or Where we temporarily drop decrypted files later on

- DataPath, or General Path To User Data

- User, or Logged in User




#### More Documentation Coming Soon!

## Roadmap

- Add more encryption systems from the XRUIOS (Such as being able to format any object when a listener hears an action and then packaging it as bytes for AES)

- Add more integrations (Browser connected Password Manager)

- Create online backup system (Send to Google Drive or another service at regular intervals)

- Implement within Spire Decentralized for Encrypted Chats, Audio, Messages and 3D Experiences

