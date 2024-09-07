# TestAssignmentLunaEdge
## Setup Instructions
1. Clone the repository: 
```bash 
git clone https://github.com/x-MiLLER-x/TestAssignmentLunaEdge.git
```
2. Navigate to the project folder: 
```bash 
cd TestAssignment
```
3. Restore dependencies: 
```bash 
dotnet restore
```
4. Install EF tools (if not installed): 
```bash 
dotnet tool install --global dotnet-ef
```
5. Apply migrations: 
```bash 
dotnet ef database update
```
6. Run the application: 
```bash 
dotnet run
```

## API Documentation
### Endpoints

1. **GET /tasks**  
   Fetch a list of tasks.  
   Optional query parameters:
   - status (TaskStatus)
   - dueDate (DateTime)
   - priority (TaskPriority)
   - pageNumber (int)
   - pageSize (int)

2. **POST /tasks**  
   Create a new task.  
   Request body (example):

   ```json
   {
     "title": "Task Title",
     "description": "Task Description",
     "dueDate": "2024-12-31T23:59:59",
     "priority": 1,
     "status": 0
   }
   ```

3. **PUT /tasks/{id}**  
   Update an existing task by ID.  
   Request body (example):

   ```json
   {
     "title": "Updated Task Title",
     "description": "Updated Task Description",
     "dueDate": "2024-12-31T23:59:59",
     "priority": 2,
     "status": 1
   }
   ```

4. **DELETE /tasks/{id}**  
   Delete a task by ID.

## Project Architecture
The project follows a layered architecture:

1. **Controllers**: Handle incoming HTTP requests and responses.
2. **Services**: Contain business logic.
3. **Repositories**: Handle data access to the database using the Repository pattern.
4. **Models**: Define the data structures and entities used throughout the application.

## Docker
To run the application using Docker, follow these steps:

1. Build the Docker image: 
```bash
docker build -t testassignment .
```
2. Run the container: 
```bash
docker run -p 5000:5000 testassignment
```

Alternatively, use the provided docker-compose.yml for setting up both the app and the database:
```bash
docker-compose up
```
