import {
  Form, Input, Select, Button, InputNumber,
  DatePicker, Typography, Card, Space,
  Tag, message, Spin, Switch,
} from 'antd'
import {
  useQuery, useMutation, useQueryClient,
} from '@tanstack/react-query'
import {
  useNavigate, useParams,
} from 'react-router-dom'
import { employeeApi } from '../../api/employees'
import {
  departmentApi, designationApi,
} from '../../api/departments'
import type { UpdateEmployeeDto } from '../../types'
import { useState, useEffect } from 'react'
import dayjs from 'dayjs'
import { ArrowLeftOutlined } from '@ant-design/icons'

const { Title } = Typography
const { Option } = Select

export default function EditEmployeePage() {
  const { id }      = useParams<{ id: string }>()
  const navigate    = useNavigate()
  const queryClient = useQueryClient()
  const [form]      = Form.useForm()
  const employeeId  = parseInt(id!)

  const [skills, setSkills]         = useState<string[]>([])
  const [skillInput, setSkillInput] = useState('')

  // ── Queries ───────────────────────────────────────────────
  const { data: employee, isLoading } = useQuery({
    queryKey: ['employee', employeeId],
    queryFn:  () => employeeApi.getById(employeeId),
  })

  const { data: departments } = useQuery({
    queryKey: ['departments'],
    queryFn:  departmentApi.getAll,
  })

  const { data: designations } = useQuery({
    queryKey: ['designations'],
    queryFn:  designationApi.getAll,
  })

  // ── Populate Form ─────────────────────────────────────────
  useEffect(() => {
    if (employee) {
      form.setFieldsValue({
        firstName:     employee.firstName,
        lastName:      employee.lastName,
        email:         employee.email,
        salary:        employee.salary,
        joiningDate:   dayjs(employee.joiningDate),
        departmentId:  employee.departmentId,
        designationId: employee.designationId,
        phone:         employee.phone,
        address:       employee.address,
        city:          employee.city,
        country:       employee.country,
        isActive:      employee.isActive,
      })
      setSkills(employee.skills)
    }
  }, [employee, form])

  // ── Mutation ──────────────────────────────────────────────
  const { mutate, isPending } = useMutation({
    mutationFn: (dto: UpdateEmployeeDto) =>
      employeeApi.update(employeeId, dto),
    onSuccess: () => {
      message.success('Employee updated successfully')
      queryClient.invalidateQueries({
        queryKey: ['employee', employeeId],
      })
      queryClient.invalidateQueries({
        queryKey: ['employees'],
      })
      navigate(`/employees/${employeeId}`)
    },
  })

  // ── Skills ────────────────────────────────────────────────
  const addSkill = () => {
    const s = skillInput.trim()
    if (s && !skills.includes(s)) {
      setSkills([...skills, s])
      setSkillInput('')
    }
  }

  // ── Submit ────────────────────────────────────────────────
  const onFinish = (values: Record<string, unknown>) => {
    const dto: UpdateEmployeeDto = {
      firstName:     values.firstName    as string,
      lastName:      values.lastName     as string,
      email:         values.email        as string,
      salary:        values.salary       as number,
      joiningDate:   dayjs(
        values.joiningDate as string)
        .format('YYYY-MM-DD'),
      departmentId:  values.departmentId  as number,
      designationId: values.designationId as number,
      phone:         values.phone         as string,
      address:       values.address       as string,
      city:          values.city          as string,
      country:       values.country       as string,
      isActive:      values.isActive      as boolean,
      skills,
    }
    mutate(dto)
  }

  if (isLoading) return (
    <div style={{ textAlign: 'center', padding: 48 }}>
      <Spin size="large" />
    </div>
  )

  return (
    <div>
      <div className="page-header">
        <Space>
          <Button
            icon={<ArrowLeftOutlined />}
            onClick={() =>
              navigate(`/employees/${employeeId}`)}
          >
            Back
          </Button>
          <Title level={3} style={{ margin: 0 }}>
            Edit Employee
          </Title>
        </Space>
      </div>

      <Card>
        <Form
          form={form}
          layout="vertical"
          onFinish={onFinish}
          style={{ maxWidth: 800 }}
        >
          <Form.Item
            name="firstName"
            label="First Name"
            rules={[{ required: true }]}
          >
            <Input />
          </Form.Item>

          <Form.Item
            name="lastName"
            label="Last Name"
            rules={[{ required: true }]}
          >
            <Input />
          </Form.Item>

          <Form.Item
            name="email"
            label="Email"
            rules={[
              { required: true },
              { type: 'email' },
            ]}
          >
            <Input />
          </Form.Item>

          <Form.Item name="phone" label="Phone">
            <Input />
          </Form.Item>

          <Form.Item name="address" label="Address">
            <Input />
          </Form.Item>

          <Space style={{ width: '100%' }} size={16}>
            <Form.Item name="city" label="City">
              <Input />
            </Form.Item>
            <Form.Item name="country" label="Country">
              <Input />
            </Form.Item>
          </Space>

          <Form.Item
            name="departmentId"
            label="Department"
            rules={[{ required: true }]}
          >
            <Select>
              {departments?.map(d => (
                <Option key={d.id} value={d.id}>
                  {d.name}
                </Option>
              ))}
            </Select>
          </Form.Item>

          <Form.Item
            name="designationId"
            label="Designation"
            rules={[{ required: true }]}
          >
            <Select>
              {designations?.map(d => (
                <Option key={d.id} value={d.id}>
                  {d.name}
                </Option>
              ))}
            </Select>
          </Form.Item>

          <Form.Item
            name="salary"
            label="Monthly Salary"
            rules={[{ required: true }]}
          >
            <InputNumber
              min={0}
              style={{ width: '100%' }}
              prefix="$"
              formatter={v =>
                `${v}`.replace(
                  /\B(?=(\d{3})+(?!\d))/g, ',')}
            />
          </Form.Item>

          <Form.Item
            name="joiningDate"
            label="Joining Date"
            rules={[{ required: true }]}
          >
            <DatePicker style={{ width: '100%' }} />
          </Form.Item>

          <Form.Item
            name="isActive"
            label="Employment Status"
            valuePropName="checked"
          >
            <Switch
              checkedChildren="Active"
              unCheckedChildren="Inactive"
            />
          </Form.Item>

          {/* Skills */}
          <Form.Item label="Skills">
            <Space direction="vertical"
              style={{ width: '100%' }}>
              <Space wrap>
                {skills.map(skill => (
                  <Tag
                    key={skill}
                    closable
                    color="blue"
                    onClose={() =>
                      setSkills(
                        skills.filter(s => s !== skill))}
                  >
                    {skill}
                  </Tag>
                ))}
              </Space>
              <Space>
                <Input
                  value={skillInput}
                  onChange={e =>
                    setSkillInput(e.target.value)}
                  placeholder="Add a skill..."
                  onPressEnter={addSkill}
                  style={{ width: 200 }}
                />
                <Button onClick={addSkill}>Add</Button>
              </Space>
            </Space>
          </Form.Item>

          <Form.Item>
            <Space>
              <Button
                type="primary"
                htmlType="submit"
                loading={isPending}
              >
                Save Changes
              </Button>
              <Button
                onClick={() =>
                  navigate(`/employees/${employeeId}`)}
              >
                Cancel
              </Button>
            </Space>
          </Form.Item>
        </Form>
      </Card>
    </div>
  )
}