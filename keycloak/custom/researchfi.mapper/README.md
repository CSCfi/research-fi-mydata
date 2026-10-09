# Username template mapper extension
This Java project produces a Keycloak username template mapper extension, which can be used to hash username coming from
external SAML IDP. Username is hashed before it is stored into Keycloak DB.

Use case is to prevent external ID to be human readable in the Keycloak user interface and database.
Project specific use case is to hash the national identification number, which is used as a persistent identifier in Suomi.fi
authentication (https://www.suomi.fi).

Implementation is done by extending this class:
https://github.com/keycloak/keycloak/blob/main/services/src/main/java/org/keycloak/broker/saml/mappers/UsernameTemplateMapper.java

# Development - Docker build
```
chmod +x build.sh
./build.sh
```

# Building and enabling a new version
The mapper is compiled against a specific Keycloak version. Rebuild it every time the Keycloak version used in the
deployment changes. The Keycloak image downloads the jar from the `master` branch of GitHub, so the jar must be
committed there before the image is built.

Paths below are relative to the repository root. Replace `X.Y.Z` with the new Keycloak version and `N` with the new mapper version.

## 1. Set the versions
Edit `keycloak/custom/researchfi.mapper/pom.xml`:
* `keycloakVersion`: set to the new Keycloak version (`X.Y.Z`).
* `<version>` (artifact version): increase it, for example `0.0.2` to `0.0.3`. Do not reuse an old version, because a
  cached Docker layer might then serve the old jar.

Do not change `PROVIDER_ID` in `ResearchfiUsernameTemplateMapper.java`. Existing identity provider mappers in the realm
refer to it.

## 2. Build the jar
Docker is the only requirement.
```
cd keycloak/custom/researchfi.mapper
rm -f researchfi-mapper.jar
./build.sh
```
How the build works: `build.sh` runs `docker build` using the `Dockerfile`, which copies `pom.xml` and `src` into a Maven
image and runs `mvn package`. The Keycloak libraries used for compiling are listed in `pom.xml` and their version is set by
`keycloakVersion` there. Maven downloads them during the build. The resulting jar contains only the mapper class, because Keycloak
provides its own libraries at runtime. `build.sh` then copies the jar out of the container as `researchfi-mapper.jar`.

The result is `researchfi-mapper.jar` in the current directory. If the file does not exist, the build failed. Look for
the Maven error in the output. A compile error usually means the Keycloak API has changed and the mapper code must be adjusted.

Optionally check the jar contents. It must contain the mapper class and `META-INF/services/org.keycloak.broker.provider.IdentityProviderMapper`:
```
unzip -l researchfi-mapper.jar
```

## 3. Add the jar to the repository
Copy the jar to the `build_dependencies` folder, using the new mapper version in the file name:
```
cp researchfi-mapper.jar ../build_dependencies/researchfi.mapper-0.0.N.jar
```

## 4. Update the Keycloak Dockerfiles
Update both files:
* `keycloak/openshift/rahti2/Dockerfile`
* `keycloak/development/Dockerfile-dev`

In each file:
* Change the Keycloak image tag to `X.Y.Z` in both `FROM keycloak/keycloak:...` lines.
* Change the jar file name to `researchfi.mapper-0.0.N.jar` in the `ADD` and `RUN chmod` lines.

## 5. Test locally
The development Dockerfile (`Dockerfile-dev`) downloads the jar from the `master` branch with an `ADD` line. To test before
merging, use the local jar instead. This is a temporary change, which must not be committed.

1. Copy the jar into the `keycloak/development` folder. Docker can only `COPY` files from the folder where the build is
   started, so the jar must be there.
   ```
   cp keycloak/custom/build_dependencies/researchfi.mapper-0.0.N.jar keycloak/development/
   ```
2. In `keycloak/development/Dockerfile-dev`, comment out the `ADD` line and add a `COPY` line below it. Leave the `RUN chmod` line as it is:
   ```
   # ADD --chown=keycloak:keycloak https://github.com/CSCfi/research-fi-mydata/raw/master/keycloak/custom/build_dependencies/researchfi.mapper-0.0.N.jar /opt/keycloak/providers/researchfi.mapper-0.0.N.jar
   COPY --chown=keycloak:keycloak researchfi.mapper-0.0.N.jar /opt/keycloak/providers/researchfi.mapper-0.0.N.jar
   RUN chmod a+r /opt/keycloak/providers/researchfi.mapper-0.0.N.jar
   ```
3. Build and start Keycloak, the database and the reverse proxy:
   ```
   cd keycloak/development
   docker compose up --build
   ```

Keycloak is available at http://localhost:8086/keycloak/. Check that:
* The image builds and Keycloak starts without errors in the log.
* In the admin console, under Server info > Providers > `identity-provider-mapper`, the provider
  `researchfi-saml-username-idp-mapper` is listed.
* The username is unchanged. Log in with the same test identifier using the old and the new Keycloak version. The created
  username must be identical (a 32 character hexadecimal MD5 hash of the upper case identifier). If it differs, existing users would
  not be recognised and duplicate users would be created.

When done, stop the containers with `docker compose down`. Then restore the `ADD` line in `Dockerfile-dev` (remove the `COPY`
line) and delete the jar copy from `keycloak/development`, so that neither is committed.

## 6. Publish and deploy
1. Back up the Keycloak database. Keycloak does not support rolling back database schema migrations.
2. Commit the new jar and the Dockerfile changes, and merge them to `master`. The jar must be on `master` before the image is built.
3. Build the Keycloak image in the devel environment and verify that logging in works. Then repeat in QA and production.

The mapper is enabled per identity provider in the Keycloak admin console. This only needs to be done for a new
identity provider. Existing mappers keep working after the upgrade.
1. Open the realm, then Identity providers > the SAML provider > Mappers > Add mapper.
2. Set the mapper type to `Researchfi Username Template Mapper` and save.