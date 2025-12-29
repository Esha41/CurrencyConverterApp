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

```bash
git clone <https://github.com/Esha41/CurrencyConverterApp.git>