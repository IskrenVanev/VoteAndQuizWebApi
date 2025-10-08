import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../AuthContext.jsx"; // Import useAuth to access login method
import './Register.css'; // Import the new CSS file

function Register() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [error, setError] = useState("");
    const navigate = useNavigate();
    const { login } = useAuth(); // Destructure login from useAuth

    const handleChange = (e) => {
        const { name, value } = e.target;
        if (name === "email") setEmail(value);
        if (name === "password") setPassword(value);
        if (name === "confirmPassword") setConfirmPassword(value);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (!email || !password || !confirmPassword) {
            setError("Please fill in all fields.");
        } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
            setError("Please enter a valid email address.");
        } else if (password !== confirmPassword) {
            setError("Passwords do not match.");
        } else {
            setError(""); // Clear error message

            try {
                const response = await fetch("https://localhost:7055/register", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: JSON.stringify({ email, password }),
                    credentials: "include", // Include cookies in the request
                });

                if (response.ok) {
                    // Log in the user immediately after registration
                    const loginResponse = await fetch("https://localhost:7055/login?useSessionCookies=true", {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json",
                        },
                        body: JSON.stringify({ email, password }),
                        credentials: "include", // Ensure cookies are sent
                    });

                    if (loginResponse.ok) {
                        await login(); // Call the login function from AuthContext
                        navigate("/"); // Redirect to home after successful login
                    } else {
                        setError("Registration successful but login failed.");
                    }
                } else {
                    const errorData = await response.json();
                    setError(errorData.message || "Error registering. Please try again.");
                }
            } catch (error) {
                setError("Error registering. Please check your network connection.");
            }
        }
    };

    const handleLoginClick = () => {
        navigate("/login");
    };

    return (
    <>
     <div className="flower-design">
            <img src="src\assets\flower.webp" alt="" />
        </div>
    <div className="register-container">
            <h3>Register</h3>
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
                <div>
                    <label htmlFor="confirmPassword">Confirm Password</label>
                    <input
                        type="password"
                        id="confirmPassword"
                        name="confirmPassword"
                        value={confirmPassword}
                        onChange={handleChange}
                        required
                    />
                </div>
                <button type="submit">Register</button>
                <button type="button" className="go-to-login-button" onClick={handleLoginClick}>
                    Go to Login
                </button>
            </form>
            {error && <p className="error">{error}</p>}
        </div>
    </>
        
    );
}

export default Register;