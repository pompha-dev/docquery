import { useState, useRef } from "react";

interface UploadDocumentProps {
  onUploadSuccess: () => void;
}

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

function UploadDocument({ onUploadSuccess }: UploadDocumentProps) {
  const [file, setFile] = useState<File | null>(null);
  const [uploadStatus, setUploadStatus] = useState("");
  const [uploading, setUploading] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const handleUpload = async () => {
    if (!file) {
      return;
    }
    setUploading(true);
    setUploadStatus("Uploading...");
    const formData = new FormData();
    formData.append("file", file);
    try {
      const response = await fetch(`${API_BASE_URL}/api/documents`, {
        method: "POST",
        body: formData,
      });

      if (!response.ok) {
        const errorMessage = await response.text();
        throw new Error(errorMessage);
      }

      const data = await response.json();
      console.log(data);
      setFile(null);
      if (fileInputRef.current) {
        fileInputRef.current.value = "";
      }
      setUploadStatus("Upload successful");
      onUploadSuccess();
    } catch (error) {
      if (error instanceof Error) {
        setUploadStatus(error.message);
      } else {
        setUploadStatus("Upload failed");
      }
    } finally {
      setUploading(false);
    }
  };

  const hanldeUploadFile = (event: React.ChangeEvent<HTMLInputElement>) => {
    const selectedFile = event.target.files?.[0] ?? null;
    setFile(selectedFile);
  };

  return (
    <div>
      <h2>Upload Document</h2>
      <input
        type="file"
        disabled={uploading}
        ref={fileInputRef}
        onChange={hanldeUploadFile}
      />
      {file && <p>Selected file: {file.name}</p>}
      {
        <button disabled={uploading} onClick={handleUpload}>
          {uploading ? "Uploading" : "Upload"}
        </button>
      }
      {uploadStatus && <p>{uploadStatus}</p>}
    </div>
  );
}

export default UploadDocument;
