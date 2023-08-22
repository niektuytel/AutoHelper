import { useAuth0 } from "@auth0/auth0-react";
import { Navigate } from "react-router-dom";

export default () => {
    const { isLoading, error } = useAuth0();

    // if (isLoading) {
    //     return <div>Loading...</div>;
    // }

    // return <Navigate to="/" />;
    
    if (error) {
      return (
        <div>
          <div className="content-layout">
            <h1 id="page-title" className="content__title">
              Error
            </h1>
            <div className="content__body">
              <p id="page-description">
                <span>{error.message}</span>
              </p>
            </div>
          </div>
        </div>
      );
    }
  
    return (
      <div className="page-layout">
        <div className="page-layout__content" />
      </div>
    );

}
