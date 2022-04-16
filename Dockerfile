# mssql-agent-fts-ha-tools
# Maintainers: Microsoft Corporation (twright-msft on GitHub)
# GitRepo: https://github.com/Microsoft/mssql-docker

# Base OS layer: Latest Ubuntu LTS
FROM ubuntu:20.04

# Install required software
RUN apt-get update && \
    export DEBIAN_FRONTEND=noninteractive && apt-get install -yq \
    curl \
    apt-transport-https \
    gnupg \
    openssh-server \
    net-tools \
    telnet \
    vim-nox \
    software-properties-common

# Install MSSQl
RUN curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - && \
    curl https://packages.microsoft.com/config/ubuntu/20.04/mssql-server-2019.list | tee /etc/apt/sources.list.d/mssql-server.list && \
    curl https://packages.microsoft.com/config/ubuntu/20.04/prod.list | tee /etc/apt/sources.list.d/msprod.list && \
    apt-get update && \
    ACCEPT_EULA=Y \
    apt-get install -yq \
    mssql-server \
    mssql-tools \
    msodbcsql18 \
    unixodbc-dev 

# Enable sshd
RUN mkdir /root/.ssh && \
    echo 'ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAABAQCN+tzy3sHQ0DM+Gax3+Acg/wfTJpd5RzB8nIfrg2sVdlLhDzC9Eq3lK+V5mAgfkVZJOnEOOviK/dqsZIudtzXGAvcAOplRLeDYeHK6wEGsSqxJkAK8UzZvKrmIlR0lhXi2Y6XhCHxoRQbOKwbNeg+qLiwQDvPrGYOt7k4c/ciszLzDibOiemztUTKo/MJCreWvhidZn4al42jzvrObWrHJcRF0rjs/mtQQcH9Vxpk/FPeq6RgrIpAtZy+Pcop2ftF22sz2SkzaxhM/sHlQf9RPyrp4mxsssT+cccvDFnrYmXrT2KIS3+xADGiSazY2RJECwGhtAQh/k9CLmaHTtkUR varga' > /root/.ssh/authorized_keys && \
    chmod 600 /root/.ssh/authorized_keys && \
    systemctl enable ssh && \
    echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> ~/.bashrc

# Install GDAL for import shape file to mssql
RUN add-apt-repository ppa:ubuntugis/ppa && \
    apt-get update && \
    apt-get install -yq gdal-bin

# Install other tools
RUN apt-get install -yq \
    pv

# Cleanup the Dockerfile
RUN apt-get clean && rm -rf /var/lib/apt/lists

# Run SQL Server process
CMD service ssh start && /opt/mssql/bin/sqlservr
