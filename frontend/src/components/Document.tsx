import { useState } from "react";

interface DocumentComponentProps {
  id: number;
  fileName: string;
  uploadedAt: string;
  onDeleteSuccess: () => void;
}

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

function DocumentComponent({
  id,
  fileName,
  uploadedAt,
  onDeleteSuccess,
}: DocumentComponentProps) {
  const [deleting, setDeleting] = useState(false);
  const [deleteError, setDeleteError] = useState<string | null>(null);

  const handleDelete = async () => {
    const confirmed = window.confirm(
      `Are you sure you want to delete "${fileName}"?`,
    );

    if (!confirmed) {
      return;
    }

    try {
      setDeleting(true);
      setDeleteError(null);

      const response = await fetch(`${API_BASE_URL}/api/documents/${id}`, {
        method: "DELETE",
        credentials: "include",
      });

      if (!response.ok) {
        console.error("Failed to delete document.");
        if (response.status === 404) {
          setDeleteError("Document not found.");
        } else {
          setDeleteError("Failed to delete document. Please try again.");
        }
        return;
      }
      onDeleteSuccess();
    } catch (error) {
      console.error("Delete request failed", error);
      setDeleteError("Unable to delete document. Please try again.");
    } finally {
      setDeleting(false);
    }
  };

  const handleOpen = async () => {
    const response = await fetch(`${API_BASE_URL}/api/documents/${id}/file`, {
      credentials: "include",
    });

    if (!response.ok) {
      throw new Error("Failed to open document.");
    }

    const blob = await response.blob();

    const url = URL.createObjectURL(blob);

    window.open(url, "_blank");
  };

  const handleDownload = async () => {
    const response = await fetch(
      `${API_BASE_URL}/api/documents/${id}/download`,
      {
        credentials: "include",
      },
    );

    if (!response.ok) {
      throw new Error("Failed to download document.");
    }

    const blob = await response.blob();

    const url = URL.createObjectURL(blob);

    const link = document.createElement("a");
    link.href = url;
    link.download = fileName;
    link.click();

    URL.revokeObjectURL(url);
  };

  return (
    <div className="document-card">
      <div className="document-info">
        <p className="document-name">{fileName}</p>

        <p className="document-date">
          {new Date(uploadedAt + "Z").toLocaleString()}
        </p>
      </div>

      {deleteError && <p className="document-error">{deleteError}</p>}

      <div className="document-actions">
        <button className="document-button" onClick={handleOpen}>
          Open
        </button>

        <button className="document-button" onClick={handleDownload}>
          Download
        </button>

        <button
          className="document-button"
          disabled={deleting}
          onClick={handleDelete}
        >
          {deleting ? "Deleting..." : "Delete"}
        </button>
      </div>
    </div>
  );
}

export default DocumentComponent;
