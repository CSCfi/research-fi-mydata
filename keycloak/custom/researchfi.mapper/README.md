# Username template mapper extension
This Java project produces a Keycloak username template mapper extension, which can be used to hash username coming from
external SAML IDP. Username is hashed before it is stored into Keycloak DB.

Use case is to prevent external ID to be human readable in the Keycloak user interface and database.
Project specific use case is to hash the national identification number, which is used as a persistent identifier in Suomi.fi
authentication (https://www.suomi.fi).

Implementation is done by extending this class:
https://github.com/keycloak/keycloak/blob/main/services/src/main/java/org/keycloak/broker/saml/mappers/UsernameTemplateMapper.java
