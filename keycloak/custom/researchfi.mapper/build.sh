#!/bin/bash

# Build the Docker image
docker build -t researchfi-mapper-builder .

# Create a temporary container (we don't need to run it)
CONTAINER_ID=$(docker create researchfi-mapper-builder)

# Copy the JAR from the container to your Macbook
docker cp $CONTAINER_ID:/app/app.jar ./researchfi-mapper.jar  # Copies to current directory, rename as needed

# Clean up: Remove the temporary container
docker rm $CONTAINER_ID

# (Optional) Remove the image if you want to save space
# docker rmi my-java-builder