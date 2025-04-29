
# BatchProcessingSystem

A complete batch-processing system built with ASP.NET Core Web APIs, RabbitMQ, SQL Server, and Docker.  
The system ingests time-stamped data, preprocesses it, aggregates results, and visualizes them via a Razor Pages dashboard.

## 🧱 Technologies Used
- ASP.NET Core 8 Web API
- Entity Framework Core
- SQL Server (Dockerized)
- RabbitMQ
- Docker & Docker Compose
- Razor Pages (Bootstrap UI)

## 🚀 How to Run

1. **Clone the repository:**
   ```bash
   git clone https://github.com/your-username/BatchProcessingSystem.git
   cd BatchProcessingSystem
   ```

2. **Build and run using Docker:**
   ```bash
   docker compose up --build
   ```

3. **Use the following endpoints in order:**

   - Start Preprocessing:
     ```
     POST http://localhost:5002/api/preprocessing/start
     ```

   - Upload CSV:
     ```
     POST http://localhost:5001/api/ingestion/upload-csv
     ```

   - Start Aggregation:
     ```
     POST http://localhost:5003/api/aggregation/aggregate
     ```

4. **View Dashboard:**
   ```
   http://localhost:5000/dashboard
   ```

## ⚠️ Note

> Default SQL Server credentials are for demo only:  
> `User Id=sa; Password=SAFAA123456`  
> Change them in production environments.
