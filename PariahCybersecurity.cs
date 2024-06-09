using System;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using Scrypt;
using DUCK.Crypto;
using Lasm.Bolt.UniversalSaver;
using System.IO;
using static XRUIOS_OS.XRUIOS.Socials;
using Unity.VisualScripting;
using static PariahCybersec.MainPasswordHandler;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.Mozilla;
using System.Collections;
using System.Threading.Tasks;

public class PariahCybersec : MonoBehaviour
{
    private static string GlobalPass; //Password
    private static string TempDecryptedFilePath; //Where we temporarily drop decrypted files
    internal static string DataPath; //General Path To User Data
    internal static string User; //Logged in User

    public class PasswordGenerator
    {
        public string CreatePassword(int length, bool IsNumeric, bool SpecialCharacters, bool Caps, bool Lowercase)
        {
            List<int> passwordchars = new List<int>(); 

            List<string> passoptions = new List<string>();

            if (IsNumeric )
            {
                passoptions.Add("IsNumeric");
            }

            if (SpecialCharacters )
            {
                passoptions.Add("SpecialCharacters");
            }

            if (Caps )
            {
                passoptions.Add("Caps");
            }

            if (Lowercase )
            {
                passoptions.Add("Lowercase");
            }

            int pcount = passoptions.Count;


                int i;

                for (i = 0; i < length;)
                {
                    i++;

                    var randitem = passoptions[RNGCSP.RollDice((byte)pcount)];

                    if (randitem == "IsNumeric")
                    {
                        passwordchars.Add(GetNumeric());
                    }

                    else if (randitem == "SpecialCharacters")
                    {
                        passwordchars.Add(GetSpecial());
                    }

                    else if (randitem == "Caps")
                    {
                    passwordchars.Add(GetUppercase());
                    }

                    else if (randitem == "Lowercase")
                    {
                    passwordchars.Add(GetLowercase());
                    }

                }


            string finalpass = default;

            foreach (int item in passwordchars)
            {
                var character = Convert.ToChar(item).ToString();

                finalpass = finalpass + character;
            }


            return finalpass;


        }


        internal int GetNumeric()
        {
            var toreturn = RNGCSP.RollDice(9) - 1 + 48;
            return toreturn;
        }

        internal int GetUppercase()
        {
            var toreturn = RNGCSP.RollDice(25) - 1 + 65;
            return toreturn;
        }

        internal int GetLowercase()
        {
            var toreturn = RNGCSP.RollDice(25) - 1 + 97;
            return toreturn;
        }

        internal int GetSpecial() 
        {
            var rand = RNGCSP.RollDice(3);

            int last = default;

            if (rand == 1)
            {
                last = RNGCSP.RollDice(14) - 1 + 33;
            }

            else if (rand == 2)
            {
                last = RNGCSP.RollDice(6) - 1 + 58;
            }
            else if (rand == 3)
            {
                last = RNGCSP.RollDice(5) - 1 + 91;
            }

            return last;
        }



    }

    public class EncryptedBoolGenerator
    {
        public struct EncryptedBoolGroup
        {
            internal List<SimpleAESEncryption.AESEncryptedText> BoolValues;
            internal List<SimpleAESEncryption.AESEncryptedText> BoolNames;
            internal SimpleAESEncryption.AESEncryptedText Trueval;
            internal SimpleAESEncryption.AESEncryptedText Falseval;

            public EncryptedBoolGroup (BoolGroup boolgroupitem)
            {
                Utilities utilistance = new Utilities();
                string trueval = utilistance.CreateUUID();
                string falseval = utilistance.CreateUUID();

                BoolValues = new List<SimpleAESEncryption.AESEncryptedText>();
                BoolNames = new List<SimpleAESEncryption.AESEncryptedText>();

                int i = 0;

                foreach (bool item in boolgroupitem.BoolValues)
                {

                    var dummystring = utilistance.GetRandomLengthString();

                    if (item == true)
                    {
                        BoolValues.Add(SimpleAESEncryption.Encrypt(trueval + dummystring + "//" + i + "//" + dummystring.Length, GlobalPass + i));
                    }

                    else if (item == false)
                    {
                        BoolValues.Add(SimpleAESEncryption.Encrypt(falseval + dummystring + "//" + i + "//" + dummystring.Length, GlobalPass + i));
                    }

                    i++;
                }

                foreach (string item in boolgroupitem.BoolNames)
                {
                    BoolNames.Add(SimpleAESEncryption.Encrypt(item, GlobalPass));
                }

                Trueval = SimpleAESEncryption.Encrypt(trueval, GlobalPass);
                Falseval = SimpleAESEncryption.Encrypt(falseval, GlobalPass);

            }
        }

        public struct BoolGroup
        {
            internal List<bool> BoolValues;
            internal List<string> BoolNames;
            internal string? TrueVal;
            internal string? FalseVal;

            public BoolGroup(List<bool> boolValues, List<string> boolNames, string? trueVal, string? falseVal)
            {
                BoolValues = boolValues;
                BoolNames = boolNames;
                TrueVal = trueVal;
                FalseVal = falseVal;
            }
        }

        public BoolGroup? DecryptBoolGroup(EncryptedBoolGroup encryptedgroup)
        {

            var trueval = SimpleAESEncryption.Decrypt(encryptedgroup.Trueval, GlobalPass);

            var falseval = SimpleAESEncryption.Decrypt(encryptedgroup.Falseval, GlobalPass);


            List<string> boolnames = new List<string>();

            int i = 0;
            foreach (SimpleAESEncryption.AESEncryptedText item in encryptedgroup.BoolNames)
            {
                boolnames.Add(SimpleAESEncryption.Decrypt(item, GlobalPass + i));
            }

            List<bool> bools = new List<bool>();

            int i2 = 0;


            foreach (SimpleAESEncryption.AESEncryptedText item in encryptedgroup.BoolValues)
            {
                

                //(trueval + dummystring + "//" + i + "//" + dummystring.Length, GlobalPass + i));

                var decryptedbool = (SimpleAESEncryption.Decrypt(item, GlobalPass + i));



                string[] parts = decryptedbool.Split(new string[] { "//" }, StringSplitOptions.None);

                string boolvalandstring = parts[0];
                int counter = int.Parse(parts[1]);
                int dummystringlengthasint = int.Parse(parts[2]);

                if (counter == i)
                {
                    //Do nothing, continue
                }

                else
                {
                    Debug.Log("The Counter Variable Does Not Match The Bool's Place, Tampered!");
                    return null;
                }

                boolvalandstring = boolvalandstring.Substring(0, boolvalandstring.Length - dummystringlengthasint);

                if (boolvalandstring == trueval)
                {
                    bools.Add(true);
                }

                else if (boolvalandstring == falseval)
                {
                    bools.Add(false);
                }

                else 
                {

                    Debug.Log("The Bool Has Been Tampered With!");
                    return null; 
                
                }

                i2++;

            }

            var finalreturn = new BoolGroup(bools, boolnames, null, null);

            return finalreturn;


        }

        public EncryptedBoolGroup? EditBoolNameInGroup (string Name, string NewName, bool NewVal, EncryptedBoolGroup encryptedgroup) 
        {
            var utilistance = new Utilities();
            var trueval = utilistance.CreateUUID();
            var falseval = utilistance.CreateUUID();

            BoolGroup decryptedgroup = (BoolGroup)DecryptBoolGroup(encryptedgroup);

            int i = 0;

            bool wasvalfound = false;

            foreach (string item in decryptedgroup.BoolNames)
            {
                if (item == Name)
                {
                    wasvalfound = true;
                    decryptedgroup.BoolNames.RemoveAt(i);
                    decryptedgroup.BoolNames.Insert(i, NewName);
                    break;
                }
                i++;
            }

            if (wasvalfound == false)
            {
                Debug.Log("The name was not found!");
                return null;
            }

            var NewBG = new EncryptedBoolGroup(decryptedgroup);
            return NewBG;

        }
        
        public EncryptedBoolGroup AddBoolToGroup(string NewName, bool NewVal, EncryptedBoolGroup encryptedgroup)
        {
            var utilistance = new Utilities();
            var trueval = utilistance.CreateUUID();
            var falseval = utilistance.CreateUUID();

            BoolGroup decryptedgroup = (BoolGroup)DecryptBoolGroup(encryptedgroup);

            
            decryptedgroup.BoolNames.Add(NewName);
            decryptedgroup.BoolValues.Add(NewVal);


            var NewBG = new EncryptedBoolGroup(decryptedgroup);
            return NewBG;

        }

        public EncryptedBoolGroup? RemoveBoolNameInGroup(string Name, bool NewVal, EncryptedBoolGroup encryptedgroup)
        {
            var utilistance = new Utilities();
            var trueval = utilistance.CreateUUID();
            var falseval = utilistance.CreateUUID();

            BoolGroup decryptedgroup = (BoolGroup)DecryptBoolGroup(encryptedgroup);

            int i = 0;

            bool wasvalfound = false;

            foreach (string item in decryptedgroup.BoolNames)
            {
                if (item == Name)
                {
                    wasvalfound = true;
                    decryptedgroup.BoolNames.RemoveAt(i);
                    decryptedgroup.BoolValues.RemoveAt(i);
                    break;
                }
                i++;
            }

            if (wasvalfound == false)
            {
                Debug.Log("The name was not found!");
                return null;
            }

            var NewBG = new EncryptedBoolGroup(decryptedgroup);
            return NewBG;

        }

        public EncryptedBoolGroup? EditBoolInGroup(string Name, bool NewVal, EncryptedBoolGroup encryptedgroup)
        {
            var utilistance = new Utilities();
            var trueval = utilistance.CreateUUID();
            var falseval = utilistance.CreateUUID();

            BoolGroup decryptedgroup = (BoolGroup)DecryptBoolGroup(encryptedgroup);

            int i = 0;

            bool wasvalfound = false;

            foreach (string item in decryptedgroup.BoolNames)
            {
                if (item == Name)
                {
                    wasvalfound = true;
                    decryptedgroup.BoolValues.RemoveAt(i);
                    decryptedgroup.BoolValues.Insert(i, NewVal);
                    break;
                }
                i++;
            }

            if (wasvalfound == false)
            {
                Debug.Log("The name was not found!");
                return null;
            }

            var NewBG = new EncryptedBoolGroup(decryptedgroup);
            return NewBG;

        }

        public bool? GetBoolValue(string Name, EncryptedBoolGroup encryptedgroup)
        {
            var utilistance = new Utilities();

            BoolGroup decryptedgroup = (BoolGroup)DecryptBoolGroup(encryptedgroup);

            bool wasvalfound = false;
            bool? boolvalue = default(bool);

            int i = 0;

            foreach (string item in decryptedgroup.BoolNames)
            {
                if (item == Name)
                {
                    wasvalfound = true;
                    boolvalue = decryptedgroup.BoolValues[i];
                    break;
                }
                i++;
            }

            if (wasvalfound == false)
            {
                Debug.Log("The name was not found!");
                return null;
            }

            return boolvalue;



        }




    }



    public class MainPasswordHandler
    {

        public struct MainPass
        {
            internal SimpleAESEncryption.AESEncryptedText UUID;
            internal string ScryptPass;

            public MainPass(SimpleAESEncryption.AESEncryptedText uuid, string scryptPass)
            {
                UUID = uuid;
                ScryptPass = scryptPass;
            }
        }


        public MainPass EncryptPassword(string Input)
        {
            ScryptEncoder encoder = new ScryptEncoder();
            var utils = new Utilities();

            var randgen = utils.CreateUUID();

            var encryptedpass = encoder.Encode(Input+randgen);

            var AESEncrypted = SimpleAESEncryption.Encrypt(randgen, Input);

            var final = new MainPass (AESEncrypted, encryptedpass);

            return final;

        }


        public static bool ValidatePass()
        {
            var logfile = Application.persistentDataPath + "//Users" + User + "GenData";

            var unisave = UniversalSave.Load(logfile, Lasm.Bolt.UniversalSaver.OdinSerializer.DataFormat.JSON);

            var se = new ScryptEncoder();

            var pass = (string)unisave.Get("Password");
            var uuid = (SimpleAESEncryption.AESEncryptedText)unisave.Get("UUID");

            string finalpass;

            try
            {
                finalpass = GlobalPass + (SimpleAESEncryption.Decrypt(uuid, GlobalPass));
            }
            catch (Exception)
            {
                return false;
            }

            var passcheck = se.Compare(finalpass, pass);

            if (!passcheck)
            {
                Debug.Log("Bad Password!");
                return false;
            }

            else
            {
                Debug.Log("Password Correct!");
            }

            return true;

        }



    }





    public class AccountEncryptor
    {

        public enum WebsiteCategory
        {
            SocialMedia,
            EmailServices,
            ECommerce,
            BankingAndFinancialServices,
            EntertainmentAndStreaming,
            EducationAndOnlineLearning,
            WorkAndProductivityTools,
            HealthcareAndMedical,
            TravelAndHospitality,
            NewsAndInformation,
            ForumsAndCommunitySites,
            GamingPlatforms,
            GovernmentAndPublicServices,
            UtilitiesAndTelecom,
            CloudStorage,
            Cryptocurrency,
            DeveloperTools,
            JobAndRecruitmentSites,
            SubscriptionsAndMemberships,
            PersonalFinance,
            Others
        }



        public struct EncryptedAccountData
        {
            internal SimpleAESEncryption.AESEncryptedText Email;
            internal SimpleAESEncryption.AESEncryptedText Username;
            internal SimpleAESEncryption.AESEncryptedText Password;
            internal EncryptedBoolGenerator.EncryptedBoolGroup Has2FA;
            internal SimpleAESEncryption.AESEncryptedText? AuthProvider;

            internal SimpleAESEncryption.AESEncryptedText CreatedOn;
            internal SimpleAESEncryption.AESEncryptedText LastEdited;
            internal SimpleAESEncryption.AESEncryptedText LastPassChange;

            internal SimpleAESEncryption.AESEncryptedText Website;
            internal SimpleAESEncryption.AESEncryptedText Notes;
            internal SimpleAESEncryption.AESEncryptedText MainLogoPath;

            internal SimpleAESEncryption.AESEncryptedText FilterType;

            public EncryptedAccountData(AccountData item)
            {
                var encryptedboolclass = new EncryptedBoolGenerator();

                Email = SimpleAESEncryption.Encrypt(item.Email, GlobalPass);
                Username = SimpleAESEncryption.Encrypt(item.Username, GlobalPass);
                Password = SimpleAESEncryption.Encrypt(item.Password, GlobalPass);


                var p1p2fa = new EncryptedBoolGenerator.BoolGroup(new List<bool> { item.Has2FA }, new List<string> { "Is2FAEnabled" }, null, null);

                Has2FA = new EncryptedBoolGenerator.EncryptedBoolGroup(p1p2fa);

                AuthProvider = SimpleAESEncryption.Encrypt(item.AuthProvider, GlobalPass);


                CreatedOn = SimpleAESEncryption.Encrypt(item.CreatedOn.ToString(), GlobalPass);
                LastEdited = SimpleAESEncryption.Encrypt(item.LastEdited.ToString(), GlobalPass);
                LastPassChange = SimpleAESEncryption.Encrypt(item.LastPassChange.ToString(), GlobalPass);

                Website = SimpleAESEncryption.Encrypt(item.Website, GlobalPass);
                Notes = SimpleAESEncryption.Encrypt(item.Notes, GlobalPass);
                MainLogoPath = SimpleAESEncryption.Encrypt(item.MainLogoPath, GlobalPass);

                FilterType = SimpleAESEncryption.Encrypt(item.FilterType.ToString(), GlobalPass);


            }

        }




        public struct AccountData
        {
            internal string Email;
            internal string Username;
            internal string Password;
            internal bool Has2FA;
            internal string AuthProvider;

            internal DateTime CreatedOn;
            internal DateTime LastEdited;
            internal DateTime LastPassChange;

            internal string Website;
            internal string Notes;
            internal string MainLogoPath;

            internal WebsiteCategory FilterType;

            
            public AccountData(
                string email,
                string username,
                string password,
                bool has2FA,
                string authProvider,
                DateTime createdOn,
                DateTime lastEdited,
                DateTime lastPassChange,
                string website,
                string notes,
                string mainLogoPath,
                WebsiteCategory filterType)
            {
                Email = email;
                Username = username;
                Password = password;
                Has2FA = has2FA;
                AuthProvider = authProvider;
                CreatedOn = createdOn;
                LastEdited = lastEdited;
                LastPassChange = lastPassChange;
                Website = website;
                Notes = notes;
                MainLogoPath = mainLogoPath;
                FilterType = filterType;
            }
        }


        public AccountData DecryptAccountData (EncryptedAccountData item)
        {

            var boolclass = new EncryptedBoolGenerator();
            

            var tempEmail = SimpleAESEncryption.Decrypt(item.Email, GlobalPass);
            var tempUsername = SimpleAESEncryption.Decrypt(item.Username, GlobalPass);
            var tempPassword = SimpleAESEncryption.Decrypt(item.Password, GlobalPass);


            var tempHas2FA = (bool)boolclass.GetBoolValue("Is2FAEnabled", item.Has2FA);


            string? tempAuthProvider;


            if (item.AuthProvider == null)
            {
                tempAuthProvider =  null;
            }

            else
            {
                tempAuthProvider = SimpleAESEncryption.Decrypt((SimpleAESEncryption.AESEncryptedText)item.AuthProvider, GlobalPass);
            }

            


            var tempCreatedOn = DateTime.Parse(SimpleAESEncryption.Decrypt(item.CreatedOn, GlobalPass));
            var tempLastEdited = DateTime.Parse(SimpleAESEncryption.Decrypt(item.LastEdited, GlobalPass));
            var tempLastPassChange = DateTime.Parse (SimpleAESEncryption.Decrypt(item.LastPassChange, GlobalPass));

            var tempWebsite = SimpleAESEncryption.Decrypt(item.Website, GlobalPass);
            var tempNotes = SimpleAESEncryption.Decrypt(item.Notes, GlobalPass);
            var tempMainLogoPath = SimpleAESEncryption.Decrypt(item.MainLogoPath, GlobalPass);

            WebsiteCategory tempFilterType = (WebsiteCategory)System.Enum.Parse(typeof(WebsiteCategory),(SimpleAESEncryption.Decrypt(item.FilterType, GlobalPass)));

            var finalreturn = new AccountData(tempEmail, tempUsername, tempPassword, tempHas2FA, tempAuthProvider, tempCreatedOn, tempLastEdited, tempLastPassChange, tempWebsite, tempNotes, tempMainLogoPath, tempFilterType);

            return finalreturn;
        }




    }

    public class FileEncrypter
    {
        public string GetEncryptedFilePath()
        {
            var unisave = UniversalSave.Load(Application.persistentDataPath + "//PariahCybersecurity" + User, Lasm.Bolt.UniversalSaver.OdinSerializer.DataFormat.JSON);
            var returnval = (string)unisave.Get("EncryptedFilePath");

            return returnval;
        }

        public void SetEncryptedFilePath(string Path)
        {
            var unisave = UniversalSave.Load(Application.persistentDataPath + "//PariahCybersecurity" + User, Lasm.Bolt.UniversalSaver.OdinSerializer.DataFormat.JSON);
            unisave.Set("EncryptedFilePath", Path);

            Debug.Log("Don't forget to move the files!");

            UniversalSave.Save(Application.persistentDataPath + "//PariahCybersecurity" + User, unisave);


        }

        public bool? CheckIfFileCanBeEncrypted(string RequestedPath)
        {
            var fileInfo = new System.IO.FileInfo("RequestedPath");

            var doesfilexist = fileInfo.Exists;

            if (doesfilexist = false)
            {
                Debug.Log("The file does not exist");
                return null;
            }


            var lengthtocheck = fileInfo.Length;
            var gblimit = 3500000000; //3.5GB in bytes



            if (lengthtocheck > gblimit)
            {
                Debug.Log("File too big");
                return false;
            }

            else
            {
                return true;
            }
        }

        public async Task<SimpleAESEncryption.AESEncryptedText> EncryptFileAsync(string RequestedPath, bool DeleteOriginalFile)
        {
            CheckIfFileCanBeEncrypted(RequestedPath);

            string fileContent = await System.IO.File.ReadAllTextAsync(RequestedPath);

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(RequestedPath);
            string originalExtension = Path.GetExtension(RequestedPath);

            var encryptiontarget = $"{fileNameWithoutExtension}||{originalExtension}||{fileContent}";

            var EncryptedFile = SimpleAESEncryption.Encrypt(fileContent, GlobalPass);

            if (DeleteOriginalFile)
            {
                System.IO.File.Delete(RequestedPath);
            }

            return EncryptedFile;
        }

        public async Task<string> DecryptFileAsync(SimpleAESEncryption.AESEncryptedText item)
        {
            var decrypteditem = SimpleAESEncryption.Decrypt(item, GlobalPass);

            string[] splitContent = decrypteditem.Split(new string[] { "||" }, 2, System.StringSplitOptions.None);
            string fileNameWithoutExtension = splitContent[0];
            string originalExtension = splitContent[1];
            string fileContent = splitContent[2];

            var finalpath = await RewriteFileIntoExistenceAsync(fileContent, fileNameWithoutExtension, originalExtension, TempDecryptedFilePath);

            return finalpath;
        }

        public async Task<string> RewriteFileIntoExistenceAsync(string base64String, string filename, string extension, string directory)
        {
            byte[] fileBytes = Convert.FromBase64String(base64String);

            string completeFilename = filename + extension;

            string fullPath = Path.Combine(directory, completeFilename);

            await File.WriteAllBytesAsync(fullPath, fileBytes);

            return fullPath;
        }


    }





    public class PasswordBank
    {

        public void CreatePB()
        {
            var newfb = new UniversalSave();
            var fblist = new List<AccountEncryptor.EncryptedAccountData>();
            newfb.Set("PB", fblist);

            UniversalSave.Save(DataPath + "//PB", newfb);
        }

        public List<AccountEncryptor.AccountData> GetAllPBInfos()
        {
            var loadedfb = UniversalSave.Load(DataPath + "//PB", Lasm.Bolt.UniversalSaver.OdinSerializer.DataFormat.JSON);
            var fblist = (List<AccountEncryptor.EncryptedAccountData>)loadedfb.Get("PB");
            var newflist = new List<AccountEncryptor.AccountData>();

            var ae = new AccountEncryptor();

            foreach (AccountEncryptor.EncryptedAccountData item in fblist)
            {
                newflist.Add(ae.DecryptAccountData(item));
            }

            return newflist;
        }

        public void AddPB(AccountEncryptor.AccountData item)
        {
            var loadedfb = UniversalSave.Load(DataPath + "//PB", Lasm.Bolt.UniversalSaver.OdinSerializer.DataFormat.JSON);
            var fblist = (List<AccountEncryptor.EncryptedAccountData>)loadedfb.Get("PB");
            var newflist = new List<AccountEncryptor.AccountData>();

            var ae = new AccountEncryptor();

            fblist.Add(new AccountEncryptor.EncryptedAccountData(item));

            UniversalSave.Save(DataPath + "//PB", loadedfb);
        }

        public AccountEncryptor.AccountData? GetFile(string Website)
        {
            var db = GetAllPBInfos();

            AccountEncryptor.AccountData? vardb = null;

            foreach (AccountEncryptor.AccountData item in db)
            {
                if (item.Website == Website)
                {
                    vardb = item;
                    break;
                }
            }

            if (vardb == null)
            {
                Debug.Log("File not found");
                return null;
            }

            return vardb;

        }

        public void UpdateInPB(string Website, AccountEncryptor.AccountData replacement)
        {
            var db = GetAllPBInfos();

            var i = 0;

            var wasitemfound = false;

            foreach (AccountEncryptor.AccountData item in db)
            {
                if (item.Website == Website)
                {
                    wasitemfound = true;
                    db.RemoveAt(i);
                    db.Insert(i, item);
                    break;
                }

                i++;
            }

            if (wasitemfound == false)
            {
                Debug.Log("Item Not Found");
                return;
            }

            var newlist = new List<AccountEncryptor.EncryptedAccountData>();

            foreach (AccountEncryptor.AccountData item in db)
            {
                newlist.Add(new AccountEncryptor.EncryptedAccountData(item));
            }

            var loadedfb = UniversalSave.Load(DataPath + "//PB", Lasm.Bolt.UniversalSaver.OdinSerializer.DataFormat.JSON);

            loadedfb.Set("PB", newlist);

            UniversalSave.Save(DataPath + "//PB", loadedfb);

        }

        public void DeleteFromDB(string Website)
        {
            var db = GetAllPBInfos();

            var i = 0;

            var wasitemfound = false;

            foreach (AccountEncryptor.AccountData item in db)
            {
                if (item.Website == Website)
                {
                    wasitemfound = true;
                    db.RemoveAt(i);
                    break;
                }

                i++;
            }

            if (wasitemfound == false)
            {
                Debug.Log("Item Not Found");
                return;
            }

            var newlist = new List<AccountEncryptor.EncryptedAccountData>();

            foreach (AccountEncryptor.AccountData item in db)
            {
                newlist.Add(new AccountEncryptor.EncryptedAccountData(item));
            }

            var loadedfb = UniversalSave.Load(DataPath + "//PB", Lasm.Bolt.UniversalSaver.OdinSerializer.DataFormat.JSON);

            loadedfb.Set("PB", newlist);

            UniversalSave.Save(DataPath + "//PB", loadedfb);

        }

        
    }


    public class AccountSystem
    {
        public void CreateAcc(string Username, string Password)
        {
            var unisave = new UniversalSave();
            unisave.Set("Username", Username);


            var MPH = new MainPasswordHandler();
            var epass = MPH.EncryptPassword(Password);


            unisave.Set("Password", epass.ScryptPass);
            unisave.Set("UUID", epass.UUID);

            DataPath = Application.persistentDataPath + "//PariahCybersecurity" + Username;
            GlobalPass = Password;

            var DR = new BackupReminder();
            DR.CreateReminderDB();

            var PB = new PasswordBank();
            PB.CreatePB();

            var FE = new FileEncrypter();
            var folder = Directory.CreateDirectory(DataPath + "//DefaultFilePath");
            var folder2 = Directory.CreateDirectory(DataPath + "//DefaultOutputFilePath");
            FE.SetEncryptedFilePath(DataPath + "//DefaultFilePath");


            UniversalSave.Save(Application.persistentDataPath + "//Users" + Username + "GenData", unisave);


        }

        public bool Login(string Username, string Password)
        {
            var logfile = Application.persistentDataPath + "//Users" + Username + "GenData";

            var unisave = UniversalSave.Load(logfile, Lasm.Bolt.UniversalSaver.OdinSerializer.DataFormat.JSON);

            var se = new ScryptEncoder();

            var pass = (string)unisave.Get("Password");
            var uuid = (SimpleAESEncryption.AESEncryptedText)unisave.Get("UUID");

            string finalpass;

            try
            {
                finalpass = GlobalPass + (SimpleAESEncryption.Decrypt(uuid, GlobalPass));
            }
            catch (Exception)
            {
                return false;
            }

            var passcheck = se.Compare(finalpass, pass);



            if (!passcheck)
            {
                Debug.Log("Bad Password!");
                return false;
            }

            else
            {
                Debug.Log("Password Correct!");
                GlobalPass = Password;
                TempDecryptedFilePath = DataPath + "//DefaultOutputFilePath";
                DataPath = Application.persistentDataPath + "//Users" + Username;
                User = Username;
            }

            return true;

        }

    }



    public class BackupReminder
    {
        public void CreateReminderDB()
        {
            var newrdb = new UniversalSave();
            newrdb.Set("LastReminder", new List<DateTime>());
            newrdb.Set("ReminderInterval", 30);

            UniversalSave.Save(DataPath + "//ReminderDB", newrdb);
        }

        public void SetReminderInDays(int Days)
        {
            var newrdb = new UniversalSave();
            newrdb.Set("ReminderInterval", Days);

            UniversalSave.Save(DataPath + "//ReminderDB", newrdb);
        }

    }


    public class Utilities
    {
        internal string CreateUUID()
        {
            string UUID = default;

            int i;

            for (i = 0; i < 20;)
            {
                var newval = RNGCSP.RollDice(9).ToString();
                UUID = string.Concat(UUID, newval);
            }

            return UUID;

        }

        internal string GetRandomLengthString()
        {
            string Val = default;

            int turns = RNGCSP.RollDice(20);

            int i;

            for (i = 0; i < turns;)
            {
                var newval = RNGCSP.RollDice(9).ToString();
                Val = string.Concat(Val, newval);
            }

            return Val;
        }



    }







    //Copied from https://gist.github.com/sachintha81/a4613d09de6b5f9d6a1a99dbf46e2385

    class RNGCSP
    {
        private static RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
        // Main method.
        public static void Main()
        {
            const int totalRolls = 25000;
            int[] results = new int[6];

            // Roll the dice 25000 times and display
            // the results to the console.
            for (int x = 0; x < totalRolls; x++)
            {
                byte roll = RollDice((byte)results.Length);
                results[roll - 1]++;
            }
            for (int i = 0; i < results.Length; ++i)
            {
                Console.WriteLine("{0}: {1} ({2:p1})", i + 1, results[i], (double)results[i] / (double)totalRolls);
            }
            rngCsp.Dispose();
            Console.ReadLine();
        }

        // This method simulates a roll of the dice. The input parameter is the
        // number of sides of the dice.

        public static byte RollDice(byte numberSides)
        {
            if (numberSides <= 0)
                throw new ArgumentOutOfRangeException("numberSides");

            // Create a byte array to hold the random value.
            byte[] randomNumber = new byte[1];
            do
            {
                // Fill the array with a random value.
                rngCsp.GetBytes(randomNumber);
            }
            while (!IsFairRoll(randomNumber[0], numberSides));
            // Return the random number mod the number
            // of sides.  The possible values are zero-
            // based, so we add one.
            return (byte)((randomNumber[0] % numberSides) + 1);
        }

        private static bool IsFairRoll(byte roll, byte numSides)
        {
            // There are MaxValue / numSides full sets of numbers that can come up
            // in a single byte.  For instance, if we have a 6 sided die, there are
            // 42 full sets of 1-6 that come up.  The 43rd set is incomplete.
            int fullSetsOfValues = Byte.MaxValue / numSides;

            // If the roll is within this range of fair values, then we let it continue.
            // In the 6 sided die case, a roll between 0 and 251 is allowed.  (We use
            // < rather than <= since the = portion allows through an extra 0 value).
            // 252 through 255 would provide an extra 0, 1, 2, 3 so they are not fair
            // to use.
            return roll < numSides * fullSetsOfValues;
        }
    }




}
