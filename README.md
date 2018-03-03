# CSGO.Report
It's a simple report bot website.

别问我为什么没有中文介绍.jpg

# Register Assistance
This tool helps you to register steam accounts quickly.

1. To use this tool with `your own email`,you must modify `MailForm.cs`,navigate to
```csharp
public ImapClient client = new ImapClient("outlook.office365.com",993,true);
```
Replace the IMAP server with yours.
```csharp
public ImapClient client = new ImapClient("Your IMAP server",Your IMAP port,true);
```
Then,scroll down and replace `report_bot@berd.moe` with your email address.You also need to replace the email in `MainForm.cs`

2. You also need to replace the group url `http://steamcommunity.com/groups/csgo_report/` with your own group url

3. Then you need an accounts collection like this:
#### Username must ends with `_ID`,`ID` must be a number
```text
PREFIX_1,YourPasswordHere,CDKEY-AAAAA-AAAAA
LOLWTF_2,YourPasswordHere,CDKEY-AAAAA-AAAAA
....
```
Just encode the collection with base64,in this example you may get following data:
```text
UFJFRklYXzEsWW91clBhc3N3b3JkSGVyZSxDREtFWS1BQUFBQS1BQUFBQQpMT0xXVEZfMixZb3VyUGFzc3dvcmRIZXJlLENES0VZLUFBQUFBLUFBQUFB
```

4. You need to create an `avatars` folder in the `root path` and put at least 1 `PNG File` into it.This program supports multiple avatars,if you add more than 1 avatar,the program will determine which avatar to use by id.

5. Now you can start the program and paste your base64 into the largest text box,input the mail password into the following small textbox and click "Continue"

6. Just input the vcode in the dialog,press `enter` and wait for automatic register

7. After the account joined your group automaticlly,click `>` button at the bottom of the program and repeat 6.
