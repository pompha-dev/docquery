import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import "../App.css";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

function RegisterPage() {
    const navigate = useNavigate();

    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(false);

    const handleRegister = async () => {
        setError("");

        if (password !== confirmPassword) {
            setError("Passwords do not match.");
            return;
        }

        setLoading(true);

        try {
            const response = await fetch(
                `${API_BASE_URL}/api/auth/register`,
                {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify({
                        email,
                        password
                    })
                }
            );

            if (!response.ok) {
                const message = await response.text();
                throw new Error(message || "Registration failed.");
            }

            navigate("/login");
        } catch (error) {
            setError(
                error instanceof Error
                    ? error.message
                    : "Something went wrong."
            );
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="auth-page">
        <div className="auth-card">
            <h1>DocQuery</h1>
            <h2>Create an account</h2>

            <input
                type="email"
                placeholder="Email"
                value={email}
                onChange={(event) => setEmail(event.target.value)}
            />

            <input
                type="password"
                placeholder="Password"
                value={password}
                onChange={(event) =>
                    setPassword(event.target.value)
                }
            />

            <input
                type="password"
                placeholder="Confirm password"
                value={confirmPassword}
                onChange={(event) =>
                    setConfirmPassword(event.target.value)
                }
            />

            {error && (
                <p className="auth-error" role="alert">
                    {error}
                </p>
            )}

            <button onClick={handleRegister} disabled={loading}>
                {loading ? "Registering..." : "Register"}
            </button>

            <p>
                Already have an account?{" "}
                <Link to="/login">Login</Link>
            </p>
        </div>
    </div>
    );
}

export default RegisterPage;