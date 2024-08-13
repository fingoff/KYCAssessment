# KYC360 Assessment
## How to Run
The project was created in VS Code with .Net 8.0. Simply clone this repo and begin debugging in VS Code (F5) to start the Swagger UI in localhost. This will show the available API endpoints created.

---

## Implementation
I have implemented **CRUD** endpoints, search with **filtering**, **pagination** and **sorting**. I have also added a **retry and backoff mechanism**

### Controller-Service-Repository
I have created a Controller-Service-Repository pattern to help separate responsibilities of my API.
- **Controller**: manages the REST interface to the business logic, in this case defines the CRUD and search endpoints.
- **Service**: implements the business logic, for the CRUD endpoints they are simply passed on to the repository, but for search, more logic is required. 
- **Repository**: handles communication with the database. For this test, we simply query our mock database. The retry and backoff mechanisms are also implemented in here for database write operations.

---

The database was mocked with an additional class "MockDatabase" which randomly generates 50 entities

### Retry and Backoff Mechanism
The Retry mechanism was created with a *RetryAsync* function in the *EntityRepository*. I used the constants *MaxRetryAttempts*, *InitialDelayMs* and *BackoffMultiplier* to tune my mechanism. They were set to 3, 500ms, and 2 respectively. An exponential backoff strategy is implemented, where the delay between each retry attempt is progressively increased. 
By using this approach, we reduce the risk of overwhelming the system in the case of a temporary issue with the network or database, improving its stability. This will in turn improve the user experience as we retry their request, up to a predefined limit.

---

Logging was also implemented with the *ILogger* interface, which helps with debugging and monitoring system behaviour. In a production environment we can structure these logs into JSON which will allow for easier parsing into a centralized logging system, in turn allowing for easier monitoring of our system.