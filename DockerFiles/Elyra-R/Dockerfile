# Base image
FROM elyra/elyra:latest

USER root

# Install R-kernel
RUN apt-get update && \
    apt-get install -y r-base && \
    R -e "install.packages(c('repr', 'IRdisplay', 'evaluate', 'crayon', 'pbdZMQ', 'devtools', 'uuid', 'digest'), repos='https://cran.rstudio.com/')"
RUN R -e "devtools::install_github('IRkernel/IRkernel')"
RUN R -e "IRkernel::installspec()"

# Switch to a non-root user
USER jovyan