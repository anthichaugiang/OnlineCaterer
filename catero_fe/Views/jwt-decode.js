function decodeToken(token) {
    try {
      const decoded = jwt.decode(token);
      return decoded;
    } catch (error) {
      console.error('Error decoding token:', error);
      return null;
    }
  }
  
  // Other code or functions in jwt-decode.js (if any)
  