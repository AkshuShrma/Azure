import { BrowserRouter, Route, Routes } from 'react-router-dom';
import './App.css';
import FileUploadSingle from './screens/FileUploadSingle';
import Login from './screens/Login';

function App() {
  return (
    <div className="App">
      <BrowserRouter>
      <Routes>
      <Route path="" element={<Login/>}/>
        <Route path="fileuplodesingle" element={<FileUploadSingle/>}/>
        <Route path="login" element={<Login/>}/>
      </Routes>
      </BrowserRouter>
    </div>
  );
}

export default App;
