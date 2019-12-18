#FROM python:3.7-alpine
FROM python:3.7-slim-stretch

WORKDIR /usr/src/mydataproject

ENV PYTHONDONTWRITEBYTECODE 1
ENV PYTHONUNBUFFERED 1

# Install unixODBC
# https://docs.microsoft.com/en-us/sql/connect/odbc/linux-mac/installing-the-microsoft-odbc-driver-for-sql-server?view=sql-server-2017
RUN apt-get update && apt-get install -y \
    curl \
    apt-transport-https \
    build-essential \
    gnupg
RUN curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add -
RUN curl https://packages.microsoft.com/config/debian/9/prod.list > /etc/apt/sources.list.d/mssql-release.list
RUN apt-get update && ACCEPT_EULA=Y apt-get -y install msodbcsql17
RUN apt-get install -y unixodbc unixodbc-dev

RUN pip install --upgrade pip
RUN pip install pipenv
COPY ./Pipfile /usr/src/mydataproject/Pipfile
RUN pipenv install --skip-lock --system --dev

COPY ./mydataproject /usr/src/mydataproject

RUN chgrp -R 0 /usr/src/mydataproject && \
    chmod -R g=u /usr/src/mydataproject && \
    chmod a+rx /usr/src/mydataproject/uid_entrypoint

RUN chmod g=u /etc/passwd
ENTRYPOINT [ "/usr/src/mydataproject/uid_entrypoint" ]
USER 1001

EXPOSE 8080

CMD ["python", "manage.py", "runserver", "0.0.0.0:8080"]