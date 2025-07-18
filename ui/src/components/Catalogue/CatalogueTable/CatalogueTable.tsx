import { useEffect, useState } from "react"
import { getBooks, updateReadStatus, type Book } from "../../../api/ApiClient";
import "./CatalogueTable.scss";

export default function CatalogueTable(props:
    {
        pageSize: string,
        sortBy: string,
        searchTerm: string
    }) {
    const [toggleButton, setToggleButton] = useState("Show details");
    const [showActions, setShowActions] = useState(true);
    const [books, setBooks] = useState<Book[]>([]);
    const [nextPageBooks, setNextPageBooks] = useState<Book[]>([]);
    const [pageNum, setPageNum] = useState("1");
    const [changeReadStatus, setChangeReadStatus] = useState("");

    useEffect(() => {
        getBooks(pageNum, props.pageSize, props.sortBy, props.searchTerm)
            .then(response => setBooks(response))
    }, [props, pageNum]);

    function prevPage(page: string) {
        let pageNum = +page;
        pageNum--;
        return pageNum.toString();
    }

    function nextPage(page: string) {
        let pageNum = +page;
        pageNum++;
        return pageNum.toString();
    }

    function checkMaxPage() {
        useEffect(() => {
            getBooks(nextPage(pageNum), props.pageSize, "Title", props.searchTerm)
                .then(response => setNextPageBooks(response));
        }, [props, pageNum]);
        return nextPageBooks.length === 0 ? true : false;
    }

    
    useEffect(() => {
        if(changeReadStatus) {
            const doUpdate = async () => {
                try {
                    await updateReadStatus(changeReadStatus);
                    await getBooks(pageNum, props.pageSize, props.sortBy, props.searchTerm)
                        .then(response => setBooks(response));
                } catch (err) {
                    console.error("Failed to update or fetch books: ", err);
                }
            };
        doUpdate();
        setChangeReadStatus("");
        }
    }, [changeReadStatus]);
    
    return (
        <div>
            <button
                onClick={() => {
                    if (toggleButton == "Show details") {
                        setToggleButton("Show actions")
                        setShowActions(false)
                    }
                    else {
                        setToggleButton("Show details")
                        setShowActions(true)
                    }
                }}>
                {toggleButton}
            </button>
            <table>
                <thead>
                    {showActions && (
                        <tr>
                            <th>ISBN</th>
                            <th>Title</th>
                            <th>Author</th>
                            <th>Collection</th>
                            <th>Publication year</th>
                        </tr>
                    )}
                    {!showActions && (
                        <tr>
                            <th>ISBN</th>
                            <th>Title</th>
                            <th>Subtitle</th>
                            <th>Author</th>
                            <th>Collection</th>
                            <th>Publication year</th>
                            <th>Edition publication year</th>
                            <th>Language</th>
                            <th>Original language</th>
                            <th>Translator</th>
                            <th>Notes</th>
                        </tr>
                    )}
                </thead>
                <tbody>
                    {books.length > 0 ? (
                        books.map(b =>
                            {return showActions ? ( 
                                <tr key={b.id} className={`${b.read ? 'marked-read' : ''}`}>
                                    <td>{b.id}</td>
                                    <td>{b.title}</td>
                                    <td>{b.author}</td>
                                    <td>{b.collection}</td>
                                    <td>{b.publicationYear}</td>
                                    <td>
                                        <button>Edit</button>
                                        <button
                                            onClick={() => {
                                                setChangeReadStatus(b.id)}}
                                        >{`${b.read ? 'Mark as unread' : 'Mark as read'}`}</button>
                                        <button>Remove</button>
                                    </td>
                                </tr>
                        ) :
                        (
                            <tr key={b.id} className={`${b.read ? 'marked-read' : ''}`}>
                                <td>{b.id}</td>
                                <td>{b.title}</td>
                                <td>{b.subtitle}</td>
                                <td>{b.author}</td>
                                <td>{b.collection}</td>
                                <td>{b.publicationYear}</td>
                                <td>{b.editionPublicationYear}</td>
                                <td>{b.language}</td>
                                <td>{b.originalLanguage}</td>
                                <td>{b.translator}</td>
                                <td>{b.notes}</td>
                                <td>
                                    <button>Edit</button>
                                </td>
                            </tr>
                        )}                        
                    )) :
                    (<tr>
                        <td colSpan={10}>No books in the library</td>
                    </tr>)}
                </tbody>
            </table>
            <button
                onClick={() => setPageNum(prevPage(pageNum))}
                disabled={pageNum === "1"}
                >
                &lt;
            </button>
            {pageNum}
            <button
                onClick={() => setPageNum(nextPage(pageNum))}
                disabled={checkMaxPage()}
                > &gt; </button>
        </div>
    )
}