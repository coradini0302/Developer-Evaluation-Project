version: '3.8'

services:
  api:
    build:
      context: .
      dockerfile: src/Ambev.DeveloperEvaluation.WebApi/Dockerfile
    ports:
      - "5000:8080"
    depends_on:
      - postgres
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=ambevdb;Username=postgres;Password=postgres

  postgres:
    image: postgres:15
    restart: always
    environment:
      POSTGRES_DB: ambevdb
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
    ports:
      - "5432:5432"