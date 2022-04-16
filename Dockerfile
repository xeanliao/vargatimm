# mssql-agent-fts-ha-tools
# Maintainers: Microsoft Corporation (twright-msft on GitHub)
# GitRepo: https://github.com/Microsoft/mssql-docker

# Base OS layer: Latest Ubuntu LTS
FROM ubuntu:20.04

# Proxy No need Out of China
RUN echo 'Acquire::http::Proxy "http://172.25.144.1:18086/";' > /etc/apt/apt.conf.d/proxy.conf
RUN echo 'Acquire::https::Proxy "http://172.25.144.1:18086/";' >> /etc/apt/apt.conf.d/proxy.conf
RUN echo '--socks5 172.25.144.1:18086' > ~/.curlrc

# Install prerequistes since it is needed to get repo config for SQL server
RUN export DEBIAN_FRONTEND=noninteractive
RUN apt-get update
RUN apt-get install -yq curl apt-transport-https gnupg

# Get official Microsoft repository configuration
RUN curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add -
RUN curl https://packages.microsoft.com/config/ubuntu/20.04/mssql-server-2019.list | tee /etc/apt/sources.list.d/mssql-server.list
RUN curl https://packages.microsoft.com/config/ubuntu/20.04/prod.list | tee /etc/apt/sources.list.d/msprod.list
RUN apt-get update

# Install SQL Server from apt
RUN apt-get install -y mssql-server

# Install optional packages
#RUN apt-get install -y mssql-server-agent
#RUN apt-get install -y mssql-server-ha
#RUN apt-get install -y mssql-server-fts

# Install mssql-tools
RUN ACCEPT_EULA=Y apt-get install -y mssql-tools unixodbc-dev

# Install sshd
RUN apt-get install -y openssh-server
RUN systemctl enable ssh
RUN mkdir /root/.ssh
RUN echo 'ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAABAQCN+tzy3sHQ0DM+Gax3+Acg/wfTJpd5RzB8nIfrg2sVdlLhDzC9Eq3lK+V5mAgfkVZJOnEOOviK/dqsZIudtzXGAvcAOplRLeDYeHK6wEGsSqxJkAK8UzZvKrmIlR0lhXi2Y6XhCHxoRQbOKwbNeg+qLiwQDvPrGYOt7k4c/ciszLzDibOiemztUTKo/MJCreWvhidZn4al42jzvrObWrHJcRF0rjs/mtQQcH9Vxpk/FPeq6RgrIpAtZy+Pcop2ftF22sz2SkzaxhM/sHlQf9RPyrp4mxsssT+cccvDFnrYmXrT2KIS3+xADGiSazY2RJECwGhtAQh/k9CLmaHTtkUR varga' > /root/.ssh/authorized_keys
RUN chmod 600 /root/.ssh/authorized_keys

# Install other software
RUN apt-get install -y net-tools
RUN apt-get install -y telnet
RUN apt-get install -y vim-nox

# Install GDAL for import shape file to mssql
RUN add-apt-repository ppa:ubuntugis/ppa
RUN apt-get update
RUN apt-get install gdal-bin

# Cleanup the Dockerfile
RUN apt-get clean
RUN rm -rf /var/lib/apt/lists
RUN rm -rf /etc/apt/apt.conf.d/proxy.conf
RUN rm -rf ~/.curlrc
RUN echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> ~/.bashrc

# Run SQL Server process
CMD service ssh start && /opt/mssql/bin/sqlservr
