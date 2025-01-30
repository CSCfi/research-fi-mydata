FROM maven:3-amazoncorretto-17 AS build

WORKDIR /app

COPY pom.xml .
COPY src ./src

RUN mvn package -DskipTests

# Final stage: Just copy the JAR.  No need for a separate JRE image if you're just copying the JAR.
FROM alpine:latest

WORKDIR /app

COPY --from=build /app/target/*.jar /app/app.jar

# No CMD needed.  We're just copying the JAR.