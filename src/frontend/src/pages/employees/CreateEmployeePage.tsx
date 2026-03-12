import {
  Form, Input, Select, Button, InputNumber,
  DatePicker, Typography, Card, Space,
  Tag, message,
} from 'antd'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { employeeApi } from '../../api/employees'
import { departmentApi, designationApi } from '../../api/departments'
import type { CreateEmployeeDto } from '../../types'
import { useState } from 'react'
import dayjs from 'dayjs'

const { Title } = Typography
const { Option } = Select

export default function CreateEmployeePage() {
  const navigate      = useNavigate()
  const queryClient   = useQueryClient()
  const [form]        = Form.useForm()
  const [skills, setSkills]     = useState<string[]>([])
  const [skillInput, setSkillInput] = useState('')

  // ── Queries ───────────────────────────────────────────────
  const { data: departments } = useQuery({
    queryKey: ['departments'],
    queryFn:  departmentApi.getAll,
  })

  const { data: designations } = useQuery({
    queryKey: ['designations'],
    queryFn:  designationApi.getAll,
  })

  // ── Mutation ──────────────────────────────────────────────
  const { mutate, isPending } = useMutation({
    mutationFn: (dto: CreateEmployeeDto) =>
      employeeApi.create(dto),
    onSuccess: (employee) => {
      message.success('Employee created successfully')
      queryClient.invalidateQueries({ queryKey: ['employees'] })
      navigate(`/employees/${employee.id}`)
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

  const removeSkill = (skill: string) => {
    setSkills(skills.filter(s => s !== skill))
  }

  // ── Submit ────────────────────────────────────────────────
  const onFinish = (values: Record<string, unknown>) => {
    const dto: CreateEmployeeDto = {
      firstName:    values.firstName    as string,
      lastName:     values.lastName     as string,
      email:        values.email        as string,
      salary:       values.salary       as number,
      joiningDate:  dayjs(values.joiningDate as string)
                      .format('YYYY-MM-DD'),
      departmentId: values.departmentId as number,
      designationId:values.designationId as number,
      phone:        values.phone        as string,
      address:      values.address      as string,
      city:         values.city         as string,
      country:      values.country      as string,
      skills,
    }
    mutate(dto)
  }

  return (
    <div>
      <div className="page-header">
        <Title level={3} style={{ margin: 0 }}>
          Add New Employee
        </Title>
        <Button onClick={() => navigate('/employees')}>
          Cancel
        </Button>
      </div>

      <Card>
        <Form
          form={form}
          layout="vertical"
          onFinish={onFinish}
          style={{ maxWidth: 800 }}
        >
          {/* Personal Info */}
          <Title level={5}>Personal Information</Title>

          <Form.Item
            name="firstName"
            label="First Name"
            rules={[{ required: true }]}
          >
            <Input placeholder="First name" />
          </Form.Item>

          <Form.Item
            name="lastName"
            label="Last Name"
            rules={[{ required: true }]}
          >
            <Input placeholder="Last name" />
          </Form.Item>

          <Form.Item
            name="email"
            label="Email"
            rules={[{ required: true }, { type: 'email' }]}
          >
            <Input placeholder="email@example.com" />
          </Form.Item>

          <Form.Item name="phone" label="Phone">
            <Input placeholder="+1 555 0000" />
          </Form.Item>

          <Form.Item name="address" label="Address">
            <Input placeholder="Street address" />
          </Form.Item>

          <Space style={{ width: '100%' }} size={16}>
            <Form.Item name="city" label="City">
              <Input placeholder="City" />
            </Form.Item>
            <Form.Item name="country" label="Country">
              <Input placeholder="Country" />
            </Form.Item>
          </Space>

          {/* Employment Info */}
          <Title level={5} style={{ marginTop: 16 }}>
            Employment Information
          </Title>

          <Form.Item
            name="departmentId"
            label="Department"
            rules={[{ required: true }]}
          >
            <Select placeholder="Select department">
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
            <Select placeholder="Select designation">
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
                `${v}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')}
            />
          </Form.Item>

          <Form.Item
            name="joiningDate"
            label="Joining Date"
            rules={[{ required: true }]}
          >
            <DatePicker style={{ width: '100%' }} />
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
                    onClose={() => removeSkill(skill)}
                  >
                    {skill}
                  </Tag>
                ))}
              </Space>
              <Space>
                <Input
                  value={skillInput}
                  onChange={e => setSkillInput(e.target.value)}
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
                Create Employee
              </Button>
              <Button onClick={() => navigate('/employees')}>
                Cancel
              </Button>
            </Space>
          </Form.Item>
        </Form>
      </Card>
    </div>
  )
}