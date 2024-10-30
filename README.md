# Alga.wwwcore

ASP.NET Core minimal api programming approach, the purpose of which is to facilitate the development of web applications and at the same time concentrate on fine-tuning the application

### Warning

Your ASP.NET Core project must include BundlerMinifier nuget package BundlerMinifier https://www.nuget.org/packages/BuildBundlerMinifier.
Reason: One of the main goals of the Alga.wwwcore package is to access a single js and css file to reduce the number of calls to the server and reduce the amount of traffic

### Updates

##### Version 1.0.10

- A critical error that was associated with the operation of the pocket in the DEBUG & RELEASE status has been fixed

##### Version 1.0.9

- The ability to add configuration rules during initialization has been added

##### Version 1.0.8

- Base directories are authomatically added to "wwwroot"