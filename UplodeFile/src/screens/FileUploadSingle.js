import axios from "axios";
import { useEffect, useState } from "react";
import * as signalR from "@microsoft/signalr";
import Header from "./Header";
import { ToastContainer, toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import swal from 'sweetalert';

function FileUploadSingle() {
  const [hubConnection, setHubConnection] = useState("");
  const [file, setFile] = useState("");
  const [data, setData] = useState([]);
  const [fileName, setFileName] = useState("");
  const [fileExtension, setFileExtension] = useState("");
  const [fileCreated, setFileCreated] = useState("");
  const [partitionKey, setPartitionkey] = useState("");
  const [rowKey, setRowkey] = useState("");
  const [userId, setUserId] = useState("");

  const [edituserId, setEditUserId] = useState("");
  const [editId, setEditId] = useState("");
  const [editpartitionKey, setEditPartitionkey] = useState("");
  const [editrowKey, setEditRowkey] = useState("");
  const [editfileCreated, setFileEditCreated] = useState("");
  const [editfileName, setEditFileName] = useState("");
  const [editfileExtension, setEditFileExtension] = useState("");

  // useEffect hook to create the connection when the component mounts
  useEffect(() => {
    getData();
    var connection = new signalR.HubConnectionBuilder()
      .withUrl(`http://localhost:5287/getDataBySignalR`)
      .build();
    //Start the connection now
    connection
      .start()
      .then(() => {
        var id = JSON.parse(localStorage.getItem("currentUser")).data.id;
        connection.invoke("UserId", id);
        console.log("connection started ");
      })
      .catch(() => {
        console.log("connection not started");
      });
    connection.on("SignalR", (connection) => {
      console.log(connection);
      setData(connection);
      setHubConnection(connection);
    });

    // cleanup function to close the connection when the component unmounts
    return () => {
      connection.stop();
    };
  }, []);

  // get All data here
  const getData = () => {
    // send the id of login user to get specific record
    const userData = JSON.parse(localStorage.getItem("currentUser"));
    axios
      .get(`http://localhost:5287/getAllData/${userData.data.id}`)
      .then((result) => {
        setData(result.data);
      })
      .catch((err) => {
        toast.dark(err);
      });
  };

  // get the data here of file
  const handleFileChange = (e) => {
    console.log(e.target.files);
    setFile(e.target.files[0]);
  };

  // Download File here
  function handleDownload(fileName) {
    let file = fileName.fileName + "." + fileName.fileExtension;
    axios
      .get("http://localhost:7000/api/DownloadImage/" + file, {
        responseType: "blob",
      })
      .then((result) => {
        swal("File Downloded Successfully",{icon:"success"});
        const url = URL.createObjectURL(new Blob([result.data]));
        const link = document.createElement("a");
        link.href = url;
        link.setAttribute("download", file);
        document.body.appendChild(link);
        link.click();
        getData();
      })
      .catch((error) => {
        toast.dark(error);
      });
  }

  // Uplode File here
  const handleUploadClick = () => {
    const userData = JSON.parse(localStorage.getItem("currentUser"));
    const header = {
      userId: userData.data.id.toString(),
    };
    const formData = new FormData();
    formData.append("file", file);
    axios
      .post("http://localhost:7000/api/FileUplode", formData, {
        headers: header,
      })
      .then((result) => {
        swal(result.data,{icon:"success"});
      })
      .catch((error) => {
        toast.dark(error);
      });
  };

  //get the value in text box
  const handleEdit = (item) => {
    setEditFileName(item.fileName);
    setEditFileExtension(item.fileExtension);
    setEditId(item.id);
    setFileEditCreated(item.fileCreated);
    setEditPartitionkey(item.partitionKey);
    setEditRowkey(item.rowKey);
    setEditUserId(item.userId);
  };

  //update record here
  const handleUpdate = async () => {
    const uRl = `http://localhost:5287/updateentityasync`;
    const data = {
      userId: edituserId,
      id: editId,
      fileCreated: editfileCreated,
      rowKey: editrowKey,
      partitionKey: editpartitionKey,
      fileName: editfileName,
      fileExtension: editfileExtension,
    };
    axios
      .put(uRl, data)
      .then((result) => {
        swal(result.data,{icon:"success"});
      })
      .catch((error) => {
        toast.dark(error);
      });
  };

  //Delete the Data here From Azure table or Azure Container
  const handleDelete = (id, partitionKey, fileExtension, fileName) => {
    debugger;
    const url = `http://localhost:5287/Delete/${partitionKey}/${id}/${fileExtension}/${fileName} `;
    const data = {
      id: id,
      fileCreated: fileCreated,
      rowKey: rowKey,
      partitionKey: partitionKey,
      fileName: fileName,
      fileExtension: fileExtension,
    };
    axios
      .delete(url, data)
      .then((result) => {
        swal(result.data,{icon:"success"});
         getData();
      })
      .catch((error) => {
        toast.dark(error);
      });
  };

  return (
    <div>
      <ToastContainer />
      <Header />
      <hr />
      {/* <div>
      <label class="form-label" for="customFile">Default file input example</label>
       <input type="file" class="text-info" />
      </div> */}
       <h2 className="text-primary text-center">Uplode Image</h2>
       <hr/>
      <div className=" col-4 offset-4">
        <div className="form-group row">
          <div className="col-8">
            <input
              type="file"
              className="form-control"
              onChange={handleFileChange}
            />
          </div>
          <button
          className="btn btn-info float-start"
          onClick={handleUploadClick}
        >
          Upload
        </button>
        </div>
        
      </div>
      <div className="row">
        <div className="col-8 text-left m-2">
          <h2 className="text-primary">Details</h2>
        </div>
        <br />
      </div>
      <div className="col-9 m-2 p-2">
        <table className="table w-75 table-bordered table-striped table-active">
          <thead>
            <tr>
              <th>FileName</th>
              <th>FileExtension</th>
              <th>CreatedDate</th>
              <th>Download</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {data && data.length > 0 ? (
              data.map((item) => {
                return (
                  <tr key={item.id}>
                    <td>{item.fileName}</td>
                    <td>{item.fileExtension}</td>
                    <td>{item.fileCreated}</td>
                    <td>
                      <button
                        className="btn btn-danger"
                        onClick={() => {
                          handleDownload(item);
                        }}
                      >
                        Download
                      </button>
                    </td>
                    <td>
                      <button
                        className="btn btn-info float-start"
                        onClick={() => handleEdit(item)}
                        data-target="#editModal"
                        data-toggle="modal"
                      >
                        Edit
                      </button>{" "}
                      &nbsp;
                      <button
                        className="btn btn-danger"
                        onClick={() =>
                          handleDelete(
                            item.id,
                            item.partitionKey,
                            item.fileExtension,
                            item.fileName
                          )
                        }
                      >
                        Delete
                      </button>
                    </td>
                  </tr>
                );
              })
            ) : (
              <tr>
                <td className="text-info">"Loading..."</td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
      <form>
        <div className="modal" id="editModal" role="dialog">
          <div className="modal-dialog">
            <div className="modal-content">
              {/* <!-- Header --> */}
              <div className="modal-header">
                <div className="modal-tittle text-primary">Edit Details</div>
                <button className="close" data-dismiss="modal">
                  <span>&times;</span>
                </button>
              </div>
              {/* <!-- Body --> */}
              <div className="modal-body">
                <div className="form-group row">
                  <label className=" text-success col-sm-4">FileName</label>
                  <div className="col-8">
                    <input
                      type="text"
                      className="form-control"
                      placeholder="Enter FileName"
                      value={editfileName}
                      onChange={(e) => setEditFileName(e.target.value)}
                    />
                  </div>
                </div>
              </div>
              {/* <!-- Footer --> */}
              <div className="modal-footer">
                <button
                  className="btn btn-info"
                  onClick={handleUpdate}
                  data-dismiss="modal"
                >
                  Submit
                </button>
                <button className="btn btn-info" data-dismiss="modal">
                  Cancel
                </button>
              </div>
            </div>
          </div>
        </div>
      </form>
    </div>
  );
}

export default FileUploadSingle;
