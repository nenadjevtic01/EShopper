import { JwtHelperService } from '@auth0/angular-jwt';

export type SessionRole = 'guest' | 'customer' | 'admin';

export function currentRole(jwtHelper: JwtHelperService): SessionRole {
  const token = localStorage.getItem('token');

  if (!token) {
    return 'guest';
  }

  try {
    if (jwtHelper.isTokenExpired(token)) {
      localStorage.removeItem('token');
      return 'guest';
    }

    const decodedToken = jwtHelper.decodeToken(token);

    return Number(decodedToken?.Role) === 1 ? 'admin' : 'customer';
  } catch {
    localStorage.removeItem('token');
    return 'guest';
  }
}
