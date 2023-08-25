import { useEffect, useContext } from 'react';
import { useAuth0 } from '@auth0/auth0-react';
import { UserContext } from '../contexts/UserContext';
import { GARAGEGUIDCLAIM, ROLESCLAIM } from '../constants/roles';

export default () => {
    const { isAuthenticated, getIdTokenClaims } = useAuth0();
    const { userRoles, setUserRoles, garageGUID, setGarageGUID } = useContext(UserContext) || {};

    useEffect(() => {
        const fetchUser = async () => {
            if (isAuthenticated) {
                try {
                    const idTokenClaims: any = await getIdTokenClaims();
                    console.log(idTokenClaims);

                    const roles = idTokenClaims[ROLESCLAIM];
                    if (setUserRoles) setUserRoles(roles);

                    const guid = idTokenClaims[GARAGEGUIDCLAIM];
                    if (setGarageGUID) {
                        setGarageGUID(guid);
                    }
                } catch (e) {
                    console.error("Error fetching role:", e);
                    // Handle error, possibly by redirecting to an error page or showing a message.
                }
            }
        };

        if (!userRoles || !garageGUID) {
            fetchUser();
        }
    }, [isAuthenticated]);

    return { userRoles, garageGUID };
};
