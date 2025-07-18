type Props = {
  getPageSize: (term: string) => void;
};

export const CataloguePageSize: React.FC<Props> = ({ getPageSize }) =>
{
  return (
    <label>
      Results per page
      <select onChange={e => getPageSize(e.target.value)}>
        <option value="25">25</option>
        <option value="50">50</option>
      </select>
    </label>
  )
}