import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from '../AuthContext.jsx'; // Import the useAuth hook
import "./Login.css";
import { Link } from "react-router-dom";

function Login() {
    const { login } = useAuth();
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [rememberme, setRememberme] = useState(false);
    const [error, setError] = useState("");

    const handleChange = (e) => {
        const { name, value, checked } = e.target;
        if (name === "email") setEmail(value);
        if (name === "password") setPassword(value);
        if (name === "rememberme") setRememberme(checked);
    };

    const handleSubmit = (e) => {
        e.preventDefault();

        if (!email || !password) {
            setError("Please fill in all fields.");
            return;
        }

        setError("");

        const loginUrl = `https://localhost:7055/login?${rememberme ? "useCookies=true" : "useSessionCookies=true"}`;
        
        fetch(loginUrl, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({ email, password }),
            credentials: "include",
        })
        .then((response) => {
            if (response.ok) {
                login();
            } else {
                setError("Error Logging In.");
            }
        })
        .catch((error) => {
            console.error("Login error:", error);
            setError("Error Logging in: " + error.message);
        });
    };

    return (
        <>
        <div className="flower-design">
            <img src="src\assets\flower.webp" alt="" />
        </div>
        <div className="login-container">
            
            <h3>Log In</h3>
            <form onSubmit={handleSubmit}>
        <div>
            <label htmlFor="email">Email</label>
            <input
                type="email"
                id="email"
                name="email"
                value={email}
                onChange={handleChange}
                required
            />
        </div>
        <div>
            <label htmlFor="password">Password</label>
            <input
                type="password"
                id="password"
                name="password"
                value={password}
                onChange={handleChange}
                required
            />
        </div>
        <div style={{ textAlign: "left", marginTop: "10px" }}>
            <input
                type="checkbox"
                id="rememberme"
                name="rememberme"
                checked={rememberme}
                onChange={handleChange}
            />
            <label htmlFor="rememberme" style={{ marginLeft: "5px" }}>
                Remember Me
            </label>
        </div>
        <div>
            <button type="submit">Log In</button>
        </div>
        {error && <p className="error">{error}</p>}

        <Link 
        to="/Register" 
        style={{ marginTop: "20px", display: "block" }}
        >
        You don't have an account? Click here.
        </Link>
    </form>

        </div>
        </>
        
    );
}

export default Login;