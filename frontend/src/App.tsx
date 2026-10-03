import { useState } from "react";
import "./App.css";
import DocumentList from "./components/DocumentList";
import UploadDocument from "./components/UploadDocument";

function App() {
  const [refresh, setRefresh] = useState(0);
  const handleRefresh = () => {
    setRefresh((prev) => prev + 1);
  };
  return (
    <div>
      <h1>DocQuery</h1>
      <p>Ask questions about your document. </p>
      <UploadDocument onUploadSuccess={handleRefresh} />
      <DocumentList
        title="My Documents"
        refresh={refresh}
        handleRefresh={handleRefresh}
      />
    </div>
  );
}

export default App;
