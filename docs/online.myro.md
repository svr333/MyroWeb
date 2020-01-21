# How Myro works

Myro is a relatively old site, with a relatively old interface.
Therefor the creation of this project. I will explain in detail how you would get a user's data.
(This is mainly for myself so I wouldn't forget it in a few months.)

## Step 1

When you go to myro, the first thing it does is creating a **GET** method to `online.myro.be/`.
This is done to obtain a PHPSESSID (a session id so it recognizes you).
You may think it'd return that as a body, but it actually gets returned as a header.

**HeaderName:** Set-Cookie  
**HeaderValue:** PHPSESSID=`{key here}`; MoreData...

To add this as a cookie in C#, you will have to, first, split all cookies (`;` splits them).
Then, split them by name & value (`=` splits them). You don't need to care about extra values that follow after the cookie's value.
[My Implementation](https://github.com/svr333/MyroWeb/blob/master/src/MyroWebClient/Extensions/CookieContainerExtensions.cs)

This will redirect you to `online.myro.be/login.php`, but this isn't a necessary step in the code.

## Step 2

The second request is a **POST** request made to `online.myro.be/loginDo.php`.
This request is a little more advanced and requires you to write data first.
Data that you need to write:

| Name  | Value |
| ------------- | ------------- |
| LoginStamp  | A stamp of the current login\*  |
| Root  | School's Abreviation  |
| Username | The user's name |
| Password | The user's password |
| Login | Log+In (this never changes) |

**Example**: LoginStamp=29797c0e+a1192b54+bf2872e8+18a14562+cfdc8d0d+aabf4e4d+58a7d6e1+2b571e95+e225ee5f&Root=SCHOOL_ABREVIATION_HERE&Username=USERNAME_HERE&Password=PASSWORD_HERE&Login=Log+in

\*Loginstamp: This looks like this: `29797c0e+a1192b54+bf2872e8+18a14562+cfdc8d0d+aabf4e4d+58a7d6e1+2b571e95+e225ee5f`,
               however, I still haven't figured out how to recreate it. Value is different on each login, though, using
               the same LoginStamp works, for now... .

The **POST** request also has a few things you need. You will need to set following headers:

| Name  | Value |
| ------------- | ------------- |
| Content-Type  | application/x-www-form-urlencoded  |
| Content-Length | BytesOfData.Length |
| Referer  | https://online.myro.be/login.php  |

After making the request, it'll create 4 cookies. It's important you carry those cookies.
Without them, you'll fail following requests.

## Step 3

The next request is a **GET** request made to `online.myro.be/index.php`.

This is a very simple request, and I have no clue what the purpose of this call is.
You post nothing, you get nothing except for a redirect to a different page.
They could've easily done that without this call.

There shouldn't be any new cookies, but catch them, just in case.

## Step 4

This is the last but also the most weird step.
This is a **GET** and a **POST** request made to `online.myro.be/logbook.php`.

We make a **GET** request, to obtain one piece of information we don't have but really need,
namely the `UserId`. We need this in order to make a **POST** request to obtain __ALL__ data,
instead of the default, the data of the current trimester.

The **GET** method is quite simple again, just make a request, check for cookies and done.
This time, the returned body (html page including js functions) does actually matter. Keep it close in a (string) var.

**This is all the data of this trimester, if that's all you need, you're done. If you need more, read along.**

In order to create the next request, you will need the `UserId`.

**UserId's** value is 2 decimals, an underscore and 4 decimals. Example: **17_3443**.
I used regex to find said UserId in the entire html file, but that's up for change.
As long as you get the id correctly 100% of the time, you're fine.

First thing you need to do, is add a cookie yourself.

| Cookie Name  | Cookie Value |
| ------------- | ------------- |
| MyroWebRapport[ShowedStudent] | `UserId` |

Don't forget to add the domain (`online.myro.be`)!
Next, you will need to create new data to write to the `online.myro.be/logbook.php`.

| Name  | Value |
| ------------- | ------------- |
| currentStudent | `UserId` |
| period`UserId` | -4096 |
| count | 10 |

**Example:** currentStudent=17_3443&period17_3443=-4096&count=10

The **POST** request also has a few things you need. You will need to set following headers:

| Name  | Value |
| ------------- | ------------- |
| Content-Type  | application/x-www-form-urlencoded  |
| Content-Length | BytesOfData.Length |
| Referer  | https://online.myro.be/login.php  |

Once you've done all this, you should be able to make the request, and the body that is returned, are all the grades of this year.

Any questions? Contact me on Discord: `svr333#3451`
