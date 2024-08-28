# NorthWindsEComm
 
> ### NOTE:
> 
> If `git` converts line endings to Windows line endings on your machine, you will need to convert the `*.sh` and `*.sql` in `NorthWindsEComm.AppHost` to Unix line endings. 
> ```powershell
> ((Get-Content $file) -join "`n") + "`n" | Set-Content -NoNewline $file
> ```

## Projects

### NorthWindsEComm.AppHost
This creates the runtime definitions for all the dependencies.

### NorthWindsEComm.ServiceDefaults
Common settings for services.

### NorthWindsEComm.CrudHelper
Common CRUD Manager, Redis Cache Access, and Data Access Interface.

### NorthWindsEComm.Gateway
Ocelot API Gateway and BFF using route aggregation.

### APIs
Microservices used for accessing the Northwinds database
- Products
- Suppliers
- Categories