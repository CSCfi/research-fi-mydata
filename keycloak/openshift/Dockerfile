#
# Build container
#
FROM --platform=linux/amd64 quay.io/keycloak/keycloak:23.0.4 as builder
ENV KC_HEALTH_ENABLED=true
ENV KC_METRICS_ENABLED=true
ENV KC_FEATURES=token-exchange
ENV KC_DB=mssql

# Install custom providers. Researchfi customization jar file should be added here.
ADD --chown=keycloak:keycloak https://github.com/CSCfi/research-fi-mydata/raw/devel/keycloak/custom/build_dependencies/researchfi.mapper-0.0.1-BETA.jar /opt/keycloak/providers/researchfi.mapper-0.0.1-BETA.jar
RUN chmod a+r /opt/keycloak/providers/researchfi.mapper-0.0.1-BETA.jar

RUN /opt/keycloak/bin/kc.sh build --transaction-xa-enabled=false




#
# Optimized container
#
FROM --platform=linux/amd64 quay.io/keycloak/keycloak:23.0.4

# Enable SHA1 to prevent JDBC error with SQL Server: "Algorithm constraints check failed on signature algorithm SHA1withRSA"
# https://github.com/keycloak/keycloak/issues/19185
USER root
RUN sed -i "s|MD2, SHA1, MD5|MD2, MD5 |g" /etc/crypto-policies/back-ends/java.config

COPY --from=builder /opt/keycloak/ /opt/keycloak/
WORKDIR /opt/keycloak

# Database connection can be specified here, but it is better to provide it from container environment. For example, as a secret in OpenShift.
# ENV KC_DB_URL=jdbc:sqlserver://myservername:1433;DatabaseName=mydatabasename;
# ENV KC_DB_USERNAME=mydatabaseusername
# ENV KC_DB_PASSWORD=mydatabasepassword

# Environment variables KC_HOSTNAME_STRICT, KC_HOSTNAME_STRICT_HTTPS, KC_PROXY and HTTP_ADDRESS_FORWARDING are needed, so that Keycloak operate behing a reverse proxy.
ENV KC_HOSTNAME_STRICT=false
ENV KC_HOSTNAME_STRICT_HTTPS=false
ENV KC_PROXY=edge
ENV HTTP_ADDRESS_FORWARDING=true
# Environment variable KC_LOG_GELF_LEVEL must be set to 'debug' in order to get Keycloak user events
ENV KC_LOG_GELF_LEVEL=debug

# Start the container in production mode.
#
# Enable backward compatibility in SAML metadata by start up parameter: keycloak.saml.deprecated.encryption=true.
# NOTE! This will not work from Keycloak 24 onwards, Suomi.fi metadata must be updated according to:
# https://www.keycloak.org/docs/latest/upgrading/index.html#saml-sp-metadata-changes
#
# Configure Keycloak log levels
# - Set INFO as default
# - Show Keycloak events in log. Included events are configured in Keycloak user interface. Log level must be DEBUG.
#     - org.keycloak.events:DEBUG
# - To reduce logging, set org.keycloak subcategories:
#     - org.keycloak.events.jpa:ERROR
#     - org.keycloak.broker:ERROR
ENTRYPOINT ["/opt/keycloak/bin/kc.sh", "start",  "--optimized", "-Dkeycloak.saml.deprecated.encryption=true", "--log-level=INFO,org.keycloak.events:DEBUG,org.keycloak.events.jpa:ERROR,org.keycloak.broker:ERROR"]