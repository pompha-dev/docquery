import Login from "../components/Login";
import "../App.css";

function LoginPage() {
  return (
    <div className="auth-page">
      <div className="auth-card">
        <h1>DocQuery</h1>
        <Login />
      </div>
    </div>
  );
}

export default LoginPage;
