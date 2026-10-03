import { useState } from "react";

interface DocumentComponentProps {
  id: number;
  fileName: string;
  uploadedAt: string;
  onDeleteSuccess: () => void;
}

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

      const response = await fetch(
        `https://localhost:7190/api/documents/${id}`,
        {
          method: "DELETE",
        },
      );

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

  const handleOpen = () => {
    window.open(`https://localhost:7190/api/documents/${id}/file`, "_blank");
  };

  const handleDownload = () => {
    window.location.href = `https://localhost:7190/api/documents/${id}/download`;
  };

  return (
    <div className="document-card">
      <p className="document-name">{fileName}</p>

      <p className="document-date">
        {new Date(uploadedAt + "Z").toLocaleString()}
      </p>

      {deleteError && <p className="document-error">{deleteError}</p>}

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
  );
}

export default DocumentComponent;
