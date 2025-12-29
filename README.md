# CurrencyConverterApp

CurrencyConverterApp is a .NET 8 Web API for fetching currency exchange rates, converting currencies, and retrieving historical exchange rate data. The application supports multiple exchange rate providers and implements role-based authorization.

## **Table of Contents**

- [Features](#features)  
- [Setup Instructions](#setup-instructions)  
- [Assumptions](#assumptions)  
- [Possible Future Enhancements](#possible-future-enhancements)  


## **Features**

- API - Fetch the latest exchange rates for a specific base currency 
- API - Convert currency amounts between two currencies  
- API - Retrieve historical exchange rates for a given period with pagination
- Implemented caching to minimize direct calls to the Frankfurter API.
- Implemented retry policies and circuit breaker to gracefully handle API outages
- Implemented dependency injection for service abstractions. 
- Designed a factory pattern to dynamically select the currency provider based on the request. 
- Allow for future integration with multiple exchange rate providers. 
- Implemented JWT authentication. 
- Enforced role-based access control (RBAC) for API endpoints. 
- Implemented API throttling to prevent abuse.
- Implemented Logging & Monitoring using serilog
- Logged all the information in a file
- Implemented API versioning for future-proofing.
- Ensured the API supports deployment in multiple environments (Dev, Test, Prod).


## **Setup Instructions**

### **1. Prerequisites**

- [.NET 8 SDK](https://dotnet.microsoft.com/download)  
- Visual Studio 2022 / VS Code (optional)  
- Internet connection for external API calls  

---

### **2. Clone the Repository**

git clone <https://github.com/Esha41/CurrencyConverterApp.git>


### **4. Restore Dependencies**

dotnet restore

### **5. Run the Application**

The API will be available at:
https://localhost:5001/api/v1/currencyconverter/


### **6. Run the Application**

Open Swagger UI in your browser:

- Use the Authorize button in Swagger to log in and test the APIs.
- Login with one of the predefined users:


| Role  | Username | Password  | Accessible Endpoints |
|-------|----------|-----------|--------------------|
| Admin | admin    | admin123  | All endpoints (latest exchange rates, historical rates, currency conversion) |
| User  | user     | user123   | Currency conversion only |


- Note: User can only access currencyConversion endpoint. Admin can access all endpoints
- Note: API versioning is implemented. Use Version '1' in order to run the existing APIs


## **Assumptions**

- The API currently supports only the Frankfurter exchange rate provider, but the architecture allows adding more providers in the future via the factory pattern.

- Roles (Admin/User) are hardcoded in appsetting.json file for demonstration; in production, roles would be managed via a database or identity provider.

- Caching is in-memory (MemoryCache) and will reset when the application restarts. Ideally redisCache should be used.

- Rate-limiting (throttling) is applied per IP address to prevent abuse; more advanced limits per client can be added later.

- Logging uses Serilog and local file logging. logs can be seen in the Logs folder which exists in the same project folder

- Distributed tracing has been implemented using OpenTelemetry. It is assumed that either the console exporter or a tracing backend (e.g., Jaeger) is available for monitoring request flows and correlating API calls.

- API versioning is currently v1, but the structure supports future versions.




## **Possible Future Enhancements**

- Add additional providers dynamically and allow user selection at runtime.

- Enhanced the API app further and use API versioning as configured in project

- Use Redis or similar to maintain cache across multiple instances for horizontal scaling.

- Make the API container-ready and deployable for horizontal scaling.

- Add real-time alerts or push notifications for significant currency rate changes

- Provide more advanced historical queries, including charts and trend analysis.
