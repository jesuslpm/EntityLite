# EntityLite

EntityLite is a lightweight, database First, micro ORM.

Read [this Code Project Article]() for an introduction to Entitylite

This package is content only, it contains T4 template and ttinclude files to generate the data layer. It references
EntityLite.Core, inercya.Newtonsoft.Json.Converters and inercya.System.Text.Json.Converters for JSON support.

Content files (T4 template and ttinclude files) are copied automatically into your project for `packages.config` projects.

However in `PackageReference` based projects content files are not copied. You need to copy then manually. They are located 
in `C:\Users\<YourUser>\.nuget\packages\entitylite\<version>\content`



