import React, { useState } from "react";
import 'bootstrap/dist/css/bootstrap.min.css';
import 'font-awesome/css/font-awesome.min.css';
import axios from 'axios';

const Auth = () => {
    const [isLogin, setIsLogin] = useState(true);
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");

    const handleSubmit = async (e) => {
        e.preventDefault();

        const user = { username, password };
        const url = isLogin
            ? "http://localhost:7052/api/auth/login"
            : "http://localhost:7052/api/auth/register";

        try {
            const response = await axios.post(url, user, {
                headers: {
                    "Content-Type": "application/json","dadad": "adada",                
                },

            });

            if (response.status === 200) {
                if (isLogin) {
                    localStorage.setItem("token", response.data.token);
                }
                alert(isLogin ? "Zalogowano!" : "Rejestracja zakończona sukcesem!");
            }
            else {
                alert(isLogin ? "NIE DZIALAAAA!" : "NIE1313123123123 zakończona sukcesem!");
            }

        } catch (error) {
            console.error("Błąd połączenia: powód", error);
            alert("Błąd połączenia z serwerem.");
        }
    };

    return (
        <div className="d-flex justify-content-center align-items-center" style={{ minHeight: "100vh", backgroundColor: "#f4f4f4" }}>
            <div className="card shadow-lg" style={{ width: "400px", borderRadius: "10px" }}>
                <div className="card-body">
                    <h2 className="text-center mb-4">{isLogin ? "Logowanie" : "Rejestracja"}</h2>
                    <form onSubmit={handleSubmit}>
                        <div className="mb-3">
                            <label htmlFor="username" className="form-label">
                                <i className="fa fa-user" aria-hidden="true"></i> Nazwa użytkownika
                            </label>
                            <input
                                type="text"
                                id="username"
                                className="form-control"
                                value={username}
                                onChange={(e) => setUsername(e.target.value)}
                                placeholder="Wpisz nazwę użytkownika"
                                required
                            />
                        </div>

                        <div className="mb-3">
                            <label htmlFor="password" className="form-label">
                                <i className="fa fa-lock" aria-hidden="true"></i> Hasło
                            </label>
                            <input
                                type="password"
                                id="password"
                                className="form-control"
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                                placeholder="Wpisz hasło"
                                required
                            />
                        </div>

                        <div className="d-grid">
                            <button type="submit" className="btn btn-primary btn-lg">
                                {isLogin ? "Zaloguj się" : "Zarejestruj się"}
                            </button>
                        </div>
                    </form>
                    <div className="text-center mt-3">
                        <button className="btn btn-link" onClick={() => setIsLogin(!isLogin)}>
                            {isLogin ? "Nie masz konta? Zarejestruj się!" : "Masz już konto? Zaloguj się!"}
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Auth;
