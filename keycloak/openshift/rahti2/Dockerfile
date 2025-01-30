#
# Build container
#
FROM --platform=linux/amd64 keycloak/keycloak:26.0.7 AS builder
ENV KC_HEALTH_ENABLED=true
ENV KC_METRICS_ENABLED=true
ENV KC_FEATURES=token-exchange
ENV KC_DB=mssql

# Install custom providers. Researchfi customization jar file should be added here.
#ADD --chown=keycloak:keycloak https://github.com/CSCfi/research-fi-mydata/raw/master/keycloak/custom/build_dependencies/researchfi.mapper-0.0.1-BETA.jar /opt/keycloak/providers/researchfi.mapper-0.0.1-BETA.jar
ADD --chown=keycloak:keycloak https://github.com/CSCfi/research-fi-mydata/raw/CSCTTV-3921-keycloak-mapper-update/keycloak/custom/build_dependencies/researchfi.mapper-0.0.2.jar /opt/keycloak/providers/researchfi.mapper-0.0.2.jar
RUN chmod a+r /opt/keycloak/providers/researchfi.mapper-0.0.2.jar

RUN /opt/keycloak/bin/kc.sh build --transaction-xa-enabled=false




#
# Optimized container
#
FROM keycloak/keycloak:26.0.7

# Enable SHA1 to prevent JDBC error with SQL Server: "Algorithm constraints check failed on signature algorithm SHA1withRSA"
# https://github.com/keycloak/keycloak/issues/19185
USER root
# Remove "SHA1" from disabledAlgorithms 
RUN sed -i "s|, SHA1,|,|g" /etc/crypto-policies/back-ends/java.config
# Remove "SHA1withRSA" from disabledAlgorithms 
RUN sed -i "s|, SHA1withRSA,|,|g" /etc/crypto-policies/back-ends/java.config

COPY --from=builder /opt/keycloak/ /opt/keycloak/
WORKDIR /opt/keycloak

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
# - Set console log timezone using %z{Europe/Helsinki}
#     - Default format is: %d{yyyy-MM-dd HH:mm:ss,SSS} %-5p [%c] (%t) %s%e%n
ENTRYPOINT ["/opt/keycloak/bin/kc.sh", "start",  "--optimized", "--log-level=INFO,org.keycloak.events:DEBUG,org.keycloak.events.jpa:ERROR,org.keycloak.broker:ERROR", "--log-console-format=%z{Europe/Helsinki}%d{yyyy-MM-dd HH:mm:ss,SSS} %-5p [%c] (%t) %s%e%n"]
