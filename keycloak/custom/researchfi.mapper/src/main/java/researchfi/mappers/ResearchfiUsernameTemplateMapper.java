/**
 * Custom username template mapper, which hashes the username
 * before it is written into Keycloak database.
 */

package researchfi.mappers;

import org.keycloak.broker.provider.BrokeredIdentityContext;
import org.keycloak.broker.saml.mappers.UsernameTemplateMapper;
import org.keycloak.models.IdentityProviderMapperModel;
import org.keycloak.models.KeycloakSession;
import org.keycloak.models.RealmModel;
import java.security.MessageDigest;
import javax.xml.bind.DatatypeConverter;

public class ResearchfiUsernameTemplateMapper extends UsernameTemplateMapper {
    public static final String PROVIDER_ID = "researchfi-saml-username-idp-mapper";

    /**
     * Use javax.xml.bind.DatatypeConverter class in JDK to convert byte array
     * to a hexadecimal string. Note that this generates hexadecimal in upper case.
     * 
     * @param hash
     * @return
     */
    private String bytesToHex(byte[] hash) {
        return DatatypeConverter.printHexBinary(hash);
    }

    /**
     * Returns a hexadecimal encoded MD5 hash for the input String.
     * 
     * @param data
     * @return
     */
    private String getMD5Hash(String data) {
        String result = null;
        try {
            MessageDigest digest = MessageDigest.getInstance("MD5");
            byte[] hash = digest.digest(data.getBytes("UTF-8"));
            return bytesToHex(hash); // make it printable
        } catch (Exception ex) {
            ex.printStackTrace();
        }
        return result;
    }

    @Override
    public String getDisplayType() {
        return "Researchfi Username Template Mapper";
    }

    @Override
    public void preprocessFederatedIdentity(KeycloakSession session, RealmModel realm,
            IdentityProviderMapperModel mapperModel, BrokeredIdentityContext context) {
        super.preprocessFederatedIdentity(session, realm, mapperModel, context);
        String username = getMD5Hash(context.getUsername());

        // Set provider user ID
        context.setId(username);

        // Set provider username
        context.setUsername(username);

        // Set local username
        context.setModelUsername(username);
    }

    @Override
    public String getId() {
        return PROVIDER_ID;
    }
}
