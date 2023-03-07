# Base image
FROM jupyter/datascience-notebook:latest

# Switch to a non-root user
USER jovyan

# Install MinIO client library
RUN pip install minio

# Install jupyterlab-quickopen-minio extension
RUN pip install jupyterlab-s3-browser

# Set environment variables for MinIO configuration
ENV AWS_S3_ENDPOINT=http://localhost:9090
ENV AWS_ACCESS_KEY_ID=abc
ENV AWS_SECRET_ACCESS_KEY=123
ENV AWS_S3_REGION=west