# Blog
A demonstration of OWASP 2017 Top 10 and .NET Core 2. This is a .net core project using an in-memory Entity Framework database and doesn't have any external
dependencies. 

# OWASP Top 10
To see the various vulnerabilites search for SECURITY or see the following list:

# A1 - Injection
Found in the class CommentsController.cs in the CREATE method. You can see ways of fixing this in the Commented out line 62 and 63
of the same class, and you can also see a parameterized way of handling this in the SQLExamples class, add method.

# A2 - Broken Authentication
Found in the class CryptographicService.cs UnsafeStorage method which stores a password using reversible encryption. Fixed in the same class
in the SafeStorage method were we properly hash the password using a key-derived function.

# A3 - Sensitive Data Exposure.
In our UserController.cs our create method helpfully is logging the fact that we are creating a user and calling ToString() on that user (Model\User.cs).  In our user class, we overrode ToString and wrote out every property helpfully formatted. Unfortunately, that causes the password to also be exposed as a 
string to our logger.

Fixed in the safeToString method in user by properly not sending the password in the return value.

# A4 - XML External Entities (XXE)
Found in our RSS Controller, Loadfeed method. This is set to process XML Entities, which means you could attempt to load aribtrary files on the servers machine and have them be returned to you as part 
of the result. This can be fixed by using DTDProcessing.Prohibit as in the Feed() method in the RSSController.

# A5 - Broken Access Control.
Our add Blog Entries method in BlogController doesn't require any authorization to post to it. We can fix this by adding an
[authorize] attribute to the method.

# A6 - Security Misconfiguration.
In Startup.cs there are two opportunities for us to setup error handling incorrectly, leaking stack traces to our users.  The first is in the Configure method
in which the useDeveloperErrorPage is set improperly. Also in that same method if the build is incorrect configured as "Developer" then the environment
won't be set correctly, also leaking errors. Finally the fact that we don't have this project setup to use HTTPS could be considered a security misconfiguration.

# A7 - Cross-Site Scripting
An example of reflected XSS is found in our index.cshtml in Views\BlogEntries\Index.cshtml reads a parameter from the query string - and injects it directly into the the dom innerHtml. Fixed in the same view by using textContent instead of innerHTml.

An example of stored XSS is found in our index.cshtml in Views\Comments\Index.cshtml where we @Html.Raw(item.Body) which, if there is a <script> 
tag embedded into the comment will load every time.

# A8 - Insecure Deserialization
An example of insecure deserialization is in our RSS Controller / CreateFeed we are processing the data as typeHandling.Auto. To resolve this, should be using typeHandling.None.

# A9 - Using Components with Known Vulnerabilities
No example present, but a good example of this to tie with A4 would be one of the services or APIs that were affected by XXE - see [The OWASP XXE .NET Cheat Sheet](https://www.owasp.org/index.php/XML_External_Entity_(XXE)_Prevention_Cheat_Sheet#.NET) for a good list of vulnerable components in the .NET framework. Also are frameworks such as Nancy, Breeze,
and DotNetNuke which at various points hav been vulnerable to A8 - Insecure Deserialization.

# A10 - Insufficient Logging & Monitoring
This is presents in usersController / delete which lets us delete users without logging access. It's fixed in 2 different ways. In the CommentsController / delete we log out there is an action occuring. The 
second way we fixed it is by adding [log4net](https://logging.apache.org/log4net/) in startup.cs. You can also seem a more comprehensive example using [Audit.Net](https://github.com/thepirat000/Audit.NET) in the CommentsController / Edit method.
 
# OWASP 2017 RC
The following examples are included for historical purposes but they were removed from the final OWASP 2017 top 10 list.

##  A7RC - Insufficient Attack Protection.
Our methods are simply rejecting invalid input in our controllers by using ModelState.IsValid checks and then just returning BadRequest. To properly
prevent attackers from keep attacking we can add (for example) a max bad request filter on a per action level. Once a limit is reached all requests will be dumped. This simple filter is implemented in the Filters\BadRequestFilter.cs and applied on UsersController.cs Delete method.

## A8RC - Cross-Site Request Forgery
Our users controller Edit method doesn't have [ValidateAntiForgeryToken] marked, making it vulnerable to a CSRF attack. This is fixed in all other
methods on any of the other actions where [ValidateAntiForgeryToken] is present.

## A10RC - Underprotected APIs
Our login method has no rate limiter. To fix this, look at the RateLimiter.cs in \RateLimiter which adds the ability to throttle connections
for a specific user. 