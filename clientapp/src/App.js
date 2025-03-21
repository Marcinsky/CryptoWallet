import React, { useState } from "react";
import Auth from "./components/Auth";
import CryptoWallet from "./components/CryptoWallet";

function App() {
    const [token, setToken] = useState(localStorage.getItem("token"));

    const handleLogout = () => {
        localStorage.removeItem("token");
        setToken(null);
    };

    return (
        <div>
            {!token ? <Auth setToken={(t) => { localStorage.setItem("token", t); setToken(t); }} />
                : <CryptoWallet />}
            {token && <button onClick={handleLogout}>Wyloguj</button>}
        </div>
    );
}

export default App;
