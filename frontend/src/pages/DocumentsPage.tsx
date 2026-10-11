import UploadDocument from "../components/UploadDocument";
import DocumentList from "../components/DocumentList";
import { useState } from "react";
import LogoutButton from "../components/LogoutButton";
import "../App.css";

function DocumentsPage() {
  const [refresh, setRefresh] = useState(0);

  const handleRefresh = () => {
    setRefresh((prev) => prev + 1);
  };

  return (
    <div className="documents-page">
      <header className="app-header">
        <h1>DocQuery</h1>
        <LogoutButton />
      </header>

      <main className="documents-content">
        <section className="upload-section">
          <h2>Upload a document</h2>
          <UploadDocument onUploadSuccess={handleRefresh} />
        </section>

        <section className="documents-section">
          <DocumentList
            title="My Documents"
            refresh={refresh}
            handleRefresh={handleRefresh}
          />
        </section>
      </main>
    </div>
  );
}

export default DocumentsPage;
