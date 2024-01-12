function getTokens(sessionKey) {
    const value = sessionStorage.getItem(sessionKey);
    if (value == null) {
        return null;
    }

    const jsonValue = JSON.parse(value);
    saveAuthState(sessionKey);
    return [jsonValue.access_token, jsonValue.id_token, jsonValue.refresh_token];
}

function getIdToken(sessionKey) {
    const value = sessionStorage.getItem(sessionKey);
    if (value == null) {
        return null;
    }

    const jsonValue = JSON.parse(value);
    return jsonValue.id_token;
}

function getRefreshToken(sessionKey) {
    const value = sessionStorage.getItem(sessionKey);
    if (value == null) {
        return null;
    }

    const jsonValue = JSON.parse(value);
    return jsonValue.refresh_token;
}

function getAccessToken(sessionKey) {
    const value = sessionStorage.getItem(sessionKey);
    if (value == null) {
        return null;
    }

    const jsonValue = JSON.parse(value);
    return jsonValue.access_token;
}

function setAuthState(sessionKey, authState) {
    removeAuthState();
    sessionStorage.setItem(sessionKey, JSON.stringify(authState));
    saveAuthState(sessionKey);
}

const AUTH_STATE_KEY = "auth_state";

function saveAuthState(sessionKey) {
    const value = sessionStorage.getItem(sessionKey);
    localStorage.setItem(
        AUTH_STATE_KEY,
        JSON.stringify(
            {
                key: sessionKey,
                value: value
            }));
}

function loadAuthState() {
    const stringValue = localStorage.getItem(AUTH_STATE_KEY);
    if (stringValue == null) {
        return false;
    }

    const value = JSON.parse(stringValue);
    const sessionValue = JSON.parse(value.value);
    const expires = new Date(sessionValue.expires_at * 1000);
    if (Date.now() >= expires) {
        removeAuthState();
        return false;
    }

    sessionStorage.setItem(value.key, value.value);
    return true;
}

function removeAuthState() {
    const stringValue = localStorage.getItem(AUTH_STATE_KEY);
    if (stringValue == null) {
        return false;
    }
    const value = JSON.parse(stringValue);
    sessionStorage.removeItem(value.key);
    localStorage.removeItem(AUTH_STATE_KEY);
}