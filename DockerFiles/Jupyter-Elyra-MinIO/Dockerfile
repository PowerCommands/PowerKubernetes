# Base image
FROM jupyter/datascience-notebook:latest

# Switch to a non-root user
USER jovyan

# Install MinIO client library
RUN pip install minio

# Install jupyterlab-quickopen-minio extension
RUN pip install jupyterlab-s3-browser
RUN  jupyter serverextension enable --py jupyterlab_s3_browser

# Set environment variables for MinIO configuration
ENV JUPYTERLAB_S3_ENDPOINT=http://localhost:9090
ENV JUPYTERLAB_S3_ACCESS_KEY_ID=abc
ENV JUPYTERLAB_S3_SECRET_ACCESS_KEY=123