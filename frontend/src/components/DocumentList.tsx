import { useEffect, useState } from "react";
import "./DocumentList.css";
import DocumentComponent from "./Document";
import type { Document } from "../types/Document";

type DocumentListProps = {
  title: string;
  refresh: number;
  handleRefresh: () => void;
};

function DocumentList({ title, refresh, handleRefresh }: DocumentListProps) {
  const [documents, setDocuments] = useState<Document[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetch("https://localhost:7190/api/documents")
      .then((response) => {
        if (!response.ok) {
          throw new Error(
            `Failed to load documents. Status: ${response.status}`,
          );
        }
        return response.json();
      })
      .then((data) => {
        setDocuments(data);
        setError(null);
      })
      .catch((error) => setError(error.message))
      .finally(() => setLoading(false));
  }, [refresh]);

  return (
    <div className="document-list">
      <h2> {title}</h2>
      {loading && <p>Loading documents</p>}
      {error && <p>{error}</p>}
      {documents.map((document) => (
        <DocumentComponent
          key={document.id}
          id={document.id}
          fileName={document.fileName}
          uploadedAt={document.uploadedAt}
          onDeleteSuccess={handleRefresh}
        />
      ))}
    </div>
  );
}

export default DocumentList;
