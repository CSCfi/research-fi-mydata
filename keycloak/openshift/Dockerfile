FROM quay.io/keycloak/keycloak:19.0.3 as builder

ENV KC_HEALTH_ENABLED=true
ENV KC_METRICS_ENABLED=true
ENV KC_FEATURES=token-exchange
ENV KC_DB=mssql
# Install custom providers
#RUN curl -sL https://github.com/aerogear/keycloak-metrics-spi/releases/download/2.5.3/keycloak-metrics-spi-2.5.3.jar -o /opt/keycloak/providers/keycloak-metrics-spi-2.5.3.jar
#COPY ttv.jar /opt/keycloak/providers/ttv.jar
RUN /opt/keycloak/bin/kc.sh build --transaction-xa-enabled=false

FROM quay.io/keycloak/keycloak:19.0.3
COPY --from=builder /opt/keycloak/ /opt/keycloak/
WORKDIR /opt/keycloak
# for demonstration purposes only, please make sure to use proper certificates in production instead
RUN keytool -genkeypair -storepass password -storetype PKCS12 -keyalg RSA -keysize 2048 -dname "CN=server" -alias server -ext "SAN:c=DNS:localhost,IP:127.0.0.1" -keystore conf/server.keystore
# change these values to point to a running postgres instance
ENV KC_HOSTNAME_STRICT=false
ENV KC_HOSTNAME_STRICT_HTTPS=false
ENV KC_PROXY=edge
ENV HTTP_ADDRESS_FORWARDING=true
ENTRYPOINT ["/opt/keycloak/bin/kc.sh", "start",  "--optimized"]
